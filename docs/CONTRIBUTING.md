# Contributing to BadNews

Thank you for your interest in contributing to BadNews! This document provides guidelines and instructions for contributing.

## Code of Conduct

Be respectful, inclusive, and professional. Treat all contributors with dignity.

## How to Contribute

### 1. Fork & Clone

```bash
# Fork the repository on GitHub
# Clone your fork
git clone https://github.com/yourusername/badnews.git
cd badnews

# Add upstream remote
git remote add upstream https://github.com/originalowner/badnews.git
```

### 2. Create Feature Branch

```bash
# Update main
git fetch upstream
git checkout main
git merge upstream/main

# Create feature branch
git checkout -b feature/your-feature-name

# Or for bug fixes
git checkout -b fix/bug-description
```

**Branch Naming Conventions:**
- Features: `feature/descriptive-name`
- Bug fixes: `fix/bug-description`
- Documentation: `docs/update-description`
- Refactoring: `refactor/change-description`

### 3. Make Changes

#### Code Style Guidelines

**Backend (C#/.NET):**
```csharp
// Classes: PascalCase
public class OrderValidator
{
    // Methods: PascalCase
    public bool ValidatePhoneNumber(string phone)
    {
        // Variables: camelCase
        var isValid = phone.StartsWith("+52");
        return isValid;
    }
}

// Async methods should end with Async
public async Task<Order> GetOrderAsync(Guid id)
{
    return await _context.Orders.FindAsync(id);
}
```

**Frontend (Vue 3):**
```vue
<!-- Components: PascalCase -->
<template>
  <div class="component-wrapper">
    <!-- Props: camelCase -->
    <CreateOrderForm @order-created="handleOrderCreated" />
  </div>
</template>

<script setup>
// Variables: camelCase
const userEmail = ref('');
const handleOrderCreated = (order) => {
  // Handle event
};
</script>
```

**Mobile (Flutter/Dart):**
```dart
// Classes: PascalCase
class OrderService {
  // Methods: camelCase
  Future<Order> getOrder(String id) async {
    // Variables: camelCase
    final order = await _api.fetchOrder(id);
    return order;
  }
}
```

#### Testing Requirements

- Backend: Minimum 80% code coverage
- Frontend: Test critical components and services
- Mobile: Test core business logic

**Backend Example:**
```csharp
[TestClass]
public class OrderValidatorTests
{
    private OrderValidator _validator;

    [TestInitialize]
    public void Setup()
    {
        _validator = new OrderValidator();
    }

    [TestMethod]
    public void ValidateMessage_WithValidInput_ReturnsTrue()
    {
        // Arrange
        var message = "This is a valid message with appropriate length and content";
        
        // Act
        var result = _validator.ValidateMessage(message);
        
        // Assert
        Assert.IsTrue(result);
    }
}
```

**Frontend Example:**
```vue
<script setup>
import { describe, it, expect } from 'vitest';
import CreateOrder from '@/components/CreateOrder.vue';
import { mount } from '@vue/test-utils';

describe('CreateOrder', () => {
  it('validates message word count', async () => {
    const wrapper = mount(CreateOrder);
    const input = wrapper.find('textarea');
    
    await input.setValue('a '.repeat(251)); // 251 words
    
    expect(wrapper.find('.word-count-warning').exists()).toBe(true);
  });
});
</script>
```

### 4. Commit Messages

**Format:**
```
type(scope): subject

body

footer
```

**Examples:**
```
feat(orders): add 250-word message validation

- Implement word count validation in validator
- Add real-time counter to frontend
- Update API documentation

Fixes #123
```

```
fix(auth): prevent token expiration race condition

Check token expiration before making API calls to prevent
concurrent requests with expired tokens.

Closes #456
```

**Types:**
- `feat`: New feature
- `fix`: Bug fix
- `docs`: Documentation
- `style`: Code style (formatting, semicolons, etc.)
- `refactor`: Code refactoring
- `test`: Adding tests
- `chore`: Build, dependencies, tooling
- `ci`: CI/CD configuration

**Scopes:**
- `orders`: Order management
- `auth`: Authentication/Authorization
- `payments`: Payment processing
- `calls`: Call management
- `ui`: User interface components
- `api`: API endpoints
- `db`: Database operations
- `mobile`: Mobile app

### 5. Testing

**Backend:**
```bash
cd backend

# Run all tests
dotnet test

# Run with coverage
dotnet test /p:CollectCoverageMetrics=true

# Run specific test
dotnet test --filter "TestClassName.TestMethodName"
```

**Frontend:**
```bash
cd frontend

# Run unit tests
npm run test

# Run with coverage
npm run test:coverage

# Run specific test file
npm run test -- CreateOrder.test.vue
```

**Mobile:**
```bash
cd mobile

# Run tests
flutter test

# Run with coverage
flutter test --coverage

# Generate coverage report
genhtml coverage/lcov.info -o coverage/html
```

### 6. Commit & Push

```bash
# Stage changes
git add .

# Commit with message
git commit -m "feat(orders): add 250-word validation"

# Push to your fork
git push origin feature/your-feature-name
```

### 7. Create Pull Request

**On GitHub:**
1. Go to the original repository
2. Click "New Pull Request"
3. Select your branch as source
4. Fill in PR template:

```markdown
## Description
Brief description of changes

## Type of Change
- [ ] Bug fix
- [ ] New feature
- [ ] Breaking change
- [ ] Documentation update

## Related Issues
Fixes #123

## Testing Done
- [ ] Unit tests added/updated
- [ ] Manual testing completed
- [ ] No breaking changes

## Screenshots (if applicable)
[Add images here]

## Checklist
- [ ] Code follows style guidelines
- [ ] Comments added for complex logic
- [ ] Documentation updated
- [ ] Tests pass locally
- [ ] No new warnings generated
```

## Review Process

### What Reviewers Look For:
- ✅ Code quality and style consistency
- ✅ Test coverage and passing tests
- ✅ Clear commit messages
- ✅ Documentation completeness
- ✅ Performance considerations
- ✅ Security best practices

### Common Feedback:

**Example: Adding Tests**
```
❌ Missing tests for edge cases
✅ Add tests for:
  - Empty message
  - Message with exactly 250 words
  - Message with special characters
```

**Example: Documentation**
```
❌ API changes not documented
✅ Please update docs/API.md with:
  - Endpoint signature
  - Request/response examples
  - Error cases
```

## Pull Request Checklist

Before submitting:
- [ ] Code follows style guidelines
- [ ] Self-reviewed the code
- [ ] Comments added for complex logic
- [ ] Documentation updated
- [ ] No new warnings generated
- [ ] Tests added/updated and passing
- [ ] Tested on Windows (if Windows-specific)
- [ ] Tested on macOS (if macOS-specific)
- [ ] Tested on Linux (if cross-platform)

## Development Setup

### Quick Start

**Backend:**
```bash
cd backend
dotnet restore
dotnet build
dotnet run
# API: http://localhost:5000
```

**Frontend:**
```bash
cd frontend
npm install
npm run dev
# Site: http://localhost:5173
```

**Mobile:**
```bash
cd mobile
flutter pub get
flutter run
```

### Database Setup

```bash
# Backend is configured to create/migrate DB on startup
# Or manually run migrations:
cd backend
dotnet ef database update
```

## Troubleshooting

### Merge Conflicts

```bash
# Update your branch
git fetch upstream
git rebase upstream/main

# Resolve conflicts in your editor
# Then continue rebase
git rebase --continue
git push -f origin feature/your-feature-name
```

### Tests Failing

```bash
# Backend
cd backend
dotnet test --verbosity detailed

# Frontend
cd frontend
npm run test -- --reporter=verbose

# Mobile
cd mobile
flutter test -v
```

## Documentation

### Adding Documentation

1. **Code Comments:** Add comments for WHY, not WHAT
   ```csharp
   // ❌ Bad: Says what the code does
   // Increment retry day
   order.RetryDay++;
   
   // ✅ Good: Explains the reason
   // Only increment retry day if we've completed all attempts for today
   if (order.DailyAttempts >= 3)
   {
       order.RetryDay++;
   }
   ```

2. **API Documentation:** Update `docs/API.md` for endpoint changes

3. **Architecture:** Update `docs/ARCHITECTURE.md` for design changes

4. **Setup:** Update `docs/SETUP.md` for configuration changes

### README Updates

If you add a feature, update relevant sections in README.md:
- Add feature description
- Update architecture diagram if needed
- Add quick start commands if applicable

## Reporting Bugs

**Open Issue:**
1. Check existing issues first
2. Use clear, descriptive title
3. Include:
   - Steps to reproduce
   - Expected behavior
   - Actual behavior
   - Environment (OS, browser, .NET version, etc.)
   - Screenshots/logs

**Example:**
```markdown
## Bug: Word counter shows incorrect count

### Steps to Reproduce
1. Go to Create Order page
2. Enter message: "This is a test message with ten words total yes"
3. Check word counter

### Expected
Shows 10 words

### Actual
Shows 9 words

### Environment
- OS: Windows 11
- Browser: Chrome 120
- Frontend Version: Latest main
```

## Feature Requests

**Open Discussion:**
1. Title: `Feature: [Brief description]`
2. Include:
   - Use case (why this is needed)
   - Proposed solution
   - Alternatives considered
   - Additional context

**Example:**
```markdown
## Feature: SMS notification when messenger is assigned

### Use Case
Buyers want to know immediately when a messenger is assigned to their order

### Proposed Solution
Send SMS via Twilio when order status changes to "Assigned"

### Implementation Notes
- Requires SMS templates in config
- Optional feature (buyer can opt-out)
```

## Release Process

When making releases:
1. Update version in `package.json` (frontend)
2. Update version in `.csproj` (backend)
3. Update `CHANGELOG.md` with changes
4. Create git tag: `git tag -a v1.2.3 -m "Release 1.2.3"`
5. Push tag: `git push origin v1.2.3`

## Getting Help

- **Questions:** Open a Discussion or ask in Discord
- **Issues:** Create an Issue
- **Reviews:** Request review from maintainers
- **Documentation:** Check `/docs` folder first

---

**Last Updated:** January 21, 2026
