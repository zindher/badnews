# API Reference

## Base URL

```
http://localhost:5000/api
```

## Authentication

All endpoints (except `/auth/register`, `/auth/login`) require:
```
Authorization: Bearer <jwt-token>
```

## Endpoints

### Auth

#### Register
```
POST /auth/register
Content-Type: application/json

{
  "email": "user@example.com",
  "password": "SecurePassword123!",
  "firstName": "John",
  "lastName": "Doe",
  "phoneNumber": "+52 555 123 4567",
  "role": "Buyer"  // or "Messenger"
}

Response: 201
{
  "userId": "550e8400-e29b-41d4-a716-446655440000",
  "email": "user@example.com",
  "role": "Buyer"
}
```

#### Login
```
POST /auth/login
Content-Type: application/json

{
  "email": "user@example.com",
  "password": "SecurePassword123!"
}

Response: 200
{
  "token": "eyJhbGciOiJIUzI1NiIs...",
  "userId": "550e8400-e29b-41d4-a716-446655440000",
  "email": "user@example.com",
  "role": "Buyer",
  "expiresIn": 86400
}
```

### Orders

#### Create Order (Buyer)
```
POST /orders
Authorization: Bearer <token>
Content-Type: application/json

{
  "recipientPhoneNumber": "+52 555 987 6543",
  "recipientName": "María",
  "recipientEmail": "maria@example.com",
  "recipientState": "Jalisco",
  "message": "¡Felicidades en tu cumpleaños María! Espero que este día sea especial...",
  "messageType": "truth",  // joke, confession, truth
  "price": 199,
  "isAnonymous": false,
  "preferredCallTime": "18:30",
  "buyerId": "550e8400-e29b-41d4-a716-446655440000"
}

Response: 201
{
  "orderId": "660e8400-e29b-41d4-a716-446655440111",
  "status": "Pending",
  "createdAt": "2026-01-21T10:30:00Z"
}
```

**Validations:**
- Message: 10-250 words max
- Price: 0 < price ≤ 10000
- Phone: Valid E.164 format
- Time: HH:MM, max 21:00
- State: Must be valid Mexican state

#### List Available Orders (Messenger)
```
GET /orders/available
Authorization: Bearer <token>

Response: 200
{
  "success": true,
  "data": [
    {
      "id": "660e8400-e29b-41d4-a716-446655440111",
      "recipientName": "María",
      "recipientPhoneNumber": "+52 555 987 6543",
      "message": "¡Felicidades en tu cumpleaños!...",
      "isAnonymous": false,
      "price": 199,
      "preferredCallTime": "18:30",
      "recipientTimezone": "CENTRO",
      "createdAt": "2026-01-21T10:30:00Z"
    }
  ]
}
```

#### Get My Orders (Buyer)
```
GET /orders/my-orders
Authorization: Bearer <token>

Response: 200
{
  "success": true,
  "data": [
    {
      "id": "660e8400-e29b-41d4-a716-446655440111",
      "recipientName": "María",
      "status": "InProgress",
      "callAttempts": 2,
      "retryDay": 0,
      "dailyAttempts": 2,
      "price": 199,
      "createdAt": "2026-01-21T10:30:00Z"
    }
  ]
}
```

#### Accept Order (Messenger)
```
PUT /orders/{orderId}/accept
Authorization: Bearer <token>

Response: 200
{
  "success": true,
  "message": "Order accepted",
  "orderId": "660e8400-e29b-41d4-a716-446655440111"
}
```

#### Rate Order (Buyer/Messenger)
```
PUT /orders/{orderId}/rate
Authorization: Bearer <token>
Content-Type: application/json

{
  "rating": 5,
  "review": "Excelente trabajo, muy profesional"
}

Response: 200
{
  "success": true,
  "message": "Order rated successfully"
}
```

### Calls

#### Make Call (Messenger)
```
POST /calls/make-call
Authorization: Bearer <token>
Content-Type: application/json

{
  "orderId": "660e8400-e29b-41d4-a716-446655440111"
}

Response: 200
{
  "success": true,
  "data": {
    "callSid": "CA1234567890abcdef1234567890abcdef",
    "orderId": "660e8400-e29b-41d4-a716-446655440111",
    "status": "Queued"
  }
}
```

#### Call Status Webhook (from Twilio)
```
POST /calls/status-callback
(No auth required - Twilio callback)

Form Data:
  CallSid: CA1234567890abcdef1234567890abcdef
  CallStatus: completed
  Duration: 120
  RecordingUrl: https://api.twilio.com/2010-04-01/Accounts/AC.../Recordings/RE...

Response: 200
{
  "success": true
}
```

### Payments

#### Create Payment
```
POST /payments/create
Authorization: Bearer <token>
Content-Type: application/json

{
  "orderId": "660e8400-e29b-41d4-a716-446655440111",
  "amount": 199,
  "paymentMethodId": "card_token_123",
  "email": "buyer@example.com"
}

Response: 201
{
  "success": true,
  "paymentId": "MP_660e8400_a1b2c3d4",
  "status": "Processing"
}
```

#### Payment Webhook (from Mercado Pago)
```
POST /payments/webhook
(No auth required - Mercado Pago callback)

Response: 200
{
  "success": true
}
```

### Messengers

#### Get Messenger Profile
```
GET /messengers/{messengerId}
Authorization: Bearer <token>

Response: 200
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "userId": "550e8400-e29b-41d4-a716-446655440001",
  "isAvailable": true,
  "rating": 4.8,
  "totalCalls": 45,
  "totalEarnings": 8955.00,
  "pendingBalance": 1500.00
}
```

#### Set Availability
```
PUT /messengers/availability
Authorization: Bearer <token>
Content-Type: application/json

{
  "isAvailable": true
}

Response: 200
{
  "success": true,
  "message": "Availability updated"
}
```

## Error Responses

### 400 Bad Request
```json
{
  "success": false,
  "message": "Message cannot exceed 250 words",
  "errors": {
    "Message": ["Message cannot exceed 250 words"]
  }
}
```

### 401 Unauthorized
```json
{
  "success": false,
  "message": "Invalid or expired token"
}
```

### 403 Forbidden
```json
{
  "success": false,
  "message": "User does not have permission"
}
```

### 404 Not Found
```json
{
  "success": false,
  "message": "Resource not found"
}
```

### 500 Internal Server Error
```json
{
  "success": false,
  "message": "Internal server error",
  "requestId": "12345"
}
```

## Rate Limiting

- Limit: 100 requests per minute per IP
- Header: `X-RateLimit-Remaining: 95`

## Pagination

```
GET /orders?page=1&pageSize=10

Response headers:
  X-Total-Count: 100
  X-Page: 1
  X-PageSize: 10
```

## Common Status Codes

| Code | Meaning |
|------|---------|
| 200 | Success |
| 201 | Created |
| 400 | Bad Request |
| 401 | Unauthorized |
| 403 | Forbidden |
| 404 | Not Found |
| 500 | Server Error |

---

**Last Updated:** January 21, 2026
