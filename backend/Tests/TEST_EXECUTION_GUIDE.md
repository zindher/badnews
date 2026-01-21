# BadNews Testing Guide

## Testing Infrastructure

This project has **two layers of automated testing**:

1. **Unit Tests (UnitTests.cs)** - 13 tests
   - AuthService, JwtService, Validators
   - Isolated business logic testing
   - Uses Moq for dependencies

2. **Integration Tests (IntegrationTests.cs)** - 30+ tests
   - Full controller workflows
   - Real database operations (InMemory)
   - End-to-end scenarios

## Running Tests

### Run All Tests
```bash
dotnet test
```

### Run Specific Test Class
```bash
dotnet test --filter "ClassName=IntegrationTests"
dotnet test --filter "ClassName=UnitTests"
```

### Run Specific Test Method
```bash
dotnet test --filter "Name~RegisterAsync_ValidRequest_CreatesUserAndReturnsSuccess"
```

### Run Tests with Verbose Output
```bash
dotnet test -v n
```

### Run Tests with Coverage (requires coverlet)
```bash
dotnet test /p:CollectCoverage=true /p:CoverageFormat=opencover
```

## Test Categories

### Authentication Tests (5 tests)
- **RegisterAsync_ValidRequest_CreatesUserAndReturnsSuccess**
  - Validates new user registration
  - Expects: 200 OK with JWT token

- **RegisterAsync_InvalidEmail_ReturnsBadRequest**
  - Tests email validation
  - Expects: 400 Bad Request

- **LoginAsync_ValidCredentials_ReturnsTokenAndUser**
  - Validates login workflow
  - Expects: 200 OK with token

- **LoginAsync_InvalidCredentials_ReturnsUnauthorized**
  - Tests credential validation
  - Expects: 401 Unauthorized

- **GetProfile_AuthorizedUser_ReturnsUserProfile**
  - Tests JWT authorization
  - Expects: 200 OK with user data

### Order Management Tests (3 tests)
- **CreateOrder_ValidRequest_CreatesOrderInDatabase**
  - Tests order creation flow
  - Requires: Buyer role, valid data

- **GetAvailableOrders_MessengerUser_ReturnsAvailableOrders**
  - Tests messenger job listing
  - Filters: Pending + Paid status

- **RateOrder_ValidRating_SavesRatingToDatabase**
  - Tests order completion workflow
  - Rating range: 1-5 stars

### Twilio Call Tests (3 tests)
- **MakeCall_ValidOrder_InitiatesTwilioCall**
  - Tests call initiation
  - Expects: Twilio SDK called with correct params

- **StatusCallback_TwilioWebhook_UpdatesCallStatus**
  - Tests webhook handling
  - Simulates Twilio status updates

- **GetCallAttempts_OrderExists_ReturnsCallHistory**
  - Tests call history retrieval
  - Shows all attempts + status

### Payment Tests (3 tests)
- **CreatePayment_ValidOrderAndAmount_CreatesPaymentRecord**
  - Tests payment creation
  - Expects: Mercado Pago API called

- **VerifyPayment_PaymentCompleted_UpdatesOrderStatus**
  - Tests payment verification
  - Updates order status when paid

- **RefundPayment_ValidPayment_ProcessesRefund**
  - Tests refund workflow
  - Updates payment status to Refunded

### Messenger Tests (1 test)
- **RegisterMessenger_ValidData_CreatesMessengerProfile**
  - Tests messenger onboarding
  - Stores bank details encrypted

- **GetMessengerStats_MessengerUser_ReturnsEarningsAndStats**
  - Tests earnings dashboard
  - Shows completed calls + earnings

### Data Persistence Tests (2 tests)
- **DatabasePersistence_MultipleOperations_MaintainsDataIntegrity**
  - Tests full workflow with all entities
  - Validates relationships

- **TransactionRollback_OnException_PreservesDataConsistency**
  - Tests rollback on failure
  - Ensures ACID compliance

## Expected Results

### Success Scenario
```
Test Run Summary:
  43 tests passed ✅
  0 tests failed
  0 tests skipped
  
Execution time: ~5 seconds
```

### Common Issues & Solutions

**Issue:** Tests fail with "DbContext already disposed"
```
Solution: Ensure DisposeAsync() is called after each test
Check: IAsyncLifetime implementation in test class
```

**Issue:** InMemory database tests pass but fail on real SQL Server
```
Solution: Real SQL Server has stricter constraint validation
Fix: Add NOT NULL constraints to test entities
```

**Issue:** Mock not being called
```
Solution: Verify mock setup matches actual parameters
Check: Times.Once() assertion, parameter matching
```

## Test Data

### Standard Test Users
- **Buyer:** buyer@example.com / SecurePassword123!
- **Messenger:** messenger@example.com / SecurePassword123!

### Test Orders
- **Basic Order:**
  - Recipient: María González
  - Phone: +34987654321
  - Price: €25.00
  - Status: Pending Payment

- **Completed Order:**
  - All fields + Rating: 5 stars
  - Status: Completed

### Test Payments
- **Card Token:** card_token_123
- **Test Amount:** €25.00
- **Mercado Pago Test Cards:**
  - Approved: 4111111111111111
  - Rejected: 4111111111111112

## Continuous Integration

### GitHub Actions Setup
Tests run automatically on:
- Every push to main
- Every pull request

### Expected Pipeline
```
1. Checkout code
2. Setup .NET 8
3. Restore packages
4. Build solution
5. Run unit tests
6. Run integration tests
7. Generate coverage report
8. Upload results
```

### Coverage Goals
- **Target:** 80%+ code coverage
- **Critical Paths:** 100% coverage (Auth, Payments, Calls)
- **Acceptable:** 70%+ for auxiliary services

## Performance Benchmarks

### Expected Test Execution Times
- Unit tests: ~0.5 seconds
- Integration tests: ~2-3 seconds
- Full suite: ~3-5 seconds

### Database Performance
- InMemory operations: <1ms
- Transaction commits: <5ms
- Rollback operations: <2ms

## Adding New Tests

### Template for Controller Tests
```csharp
[Fact]
public async Task MethodName_Scenario_ExpectedResult()
{
    // Arrange - Setup test data and mocks
    var request = new SomeRequest { /* data */ };
    _mockService.Setup(s => s.Method(It.IsAny<T>()))
        .ReturnsAsync(expectedValue);
    
    var controller = new SomeController(_mockService.Object, _dbContext, _logger);
    
    // Act - Execute the test
    var result = await controller.MethodName(request);
    
    // Assert - Verify outcomes
    var okResult = Assert.IsType<OkObjectResult>(result);
    Assert.Equal(200, okResult.StatusCode);
    _mockService.Verify(s => s.Method(It.IsAny<T>()), Times.Once);
}
```

## Debugging Tests

### Enable Debug Output
```bash
dotnet test --logger:"console;verbosity=detailed"
```

### Attach Debugger
```bash
dotnet test --no-build -- --debug
```

### Visual Studio Test Explorer
1. Test Explorer → Select test
2. Right-click → Debug Selected Tests
3. Breakpoints work normally

## Troubleshooting

### Tests Timeout
```
Increase timeout in test runner configuration
Use test.timeout = 10000 in xunit.runner.json
```

### Database Lock Issues
```
InMemory database doesn't support locks
Use real SQL Server for concurrent tests
```

### Mock Verification Fails
```
Verify parameter matching:
  - Use It.IsAny<T>() for flexible matching
  - Use It.Is<T>(x => x.Property == value) for specific matching
```

## Test Maintenance

### Regular Tasks
- [ ] Run full test suite weekly
- [ ] Update tests when adding features
- [ ] Keep test data synchronized
- [ ] Review coverage reports monthly

### When Tests Break
1. Check if code change was intentional
2. Update test assertions if behavior changed
3. Add new tests for new functionality
4. Never disable failing tests without investigation

