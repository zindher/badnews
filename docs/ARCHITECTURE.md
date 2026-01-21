# System Architecture

## Overview

Gritalo is a 3-tier distributed system:

```
┌─────────────────────┐
│   Vue 3 Frontend    │
│    (5173)           │
└──────────┬──────────┘
           │ HTTP/REST
┌──────────▼──────────┐
│  .NET Core Backend  │
│    (5000)           │
└──────────┬──────────┘
           │ EF Core
┌──────────▼──────────┐
│   SQL Server DB     │
│  GritaloDb          │
└─────────────────────┘
```

## Backend Architecture

### Layer Design

```
Controllers (API Layer)
    ↓
Services (Business Logic)
    ↓
Data Context (ORM/EF Core)
    ↓
SQL Server Database
```

### Key Components

#### Models (`Models/`)
- `User` - Compradores y Mensajeros
- `Order` - Encargos con estado y reintentos
- `Payment` - Pagos con Mercado Pago
- `CallAttempt` - Historial de llamadas
- `Messenger` - Perfil de mensajero

#### Controllers (`Controllers/`)
- `AuthController` - JWT authentication
- `OrdersController` - CRUD de órdenes
- `CallsController` - Llamadas y grabaciones
- `PaymentsController` - Integración Mercado Pago
- `MessengersController` - Perfil de mensajeros

#### Services (`Services/`)
- `TwilioServiceImpl` - Integración Twilio (llamadas, SMS)
- `SendGridServiceImpl` - Notificaciones email
- `MercadoPagoServiceImpl` - Pagos
- `OrderService` - Lógica de órdenes
- `TimezoneService` - Conversión de zonas horarias

#### Jobs (`Jobs/`)
- `CallRetryJob` - Sistema 3x3 de reintentos
  - 3 llamadas/día × 3 días = 9 intentos
  - SMS/Email fallback
  - Refund automático
  - Ejecutado con Hangfire

#### Validators (`Validators/`)
- FluentValidation para todas las DTOs
- Validación de palabras (máx 250)
- Validación de timezones
- Validación de formularios

### Database Schema

```
┌─────────────┐         ┌────────────┐
│    Users    │◄────────│   Orders   │
├─────────────┤         ├────────────┤
│ Id (PK)     │         │ Id (PK)    │
│ Email       │         │ BuyerId(FK)│
│ PasswordHash│         │ MessengerId│
│ Role        │         │ Status     │
└─────────────┘         │ RetryDay   │
                        │ CallAttempts
                        └────────────┘
                              │
                        ┌─────▼─────────┐
                        │ CallAttempts  │
                        ├───────────────┤
                        │ OrderId (FK)  │
                        │ AttemptNumber │
                        │ Status        │
                        │ TwilioCallSid │
                        │ RecordingUrl  │
                        └───────────────┘

┌──────────┐           ┌─────────┐
│ Payments │───────────│ Orders  │
├──────────┤           └─────────┘
│ OrderId  │
│ Status   │
│ Amount   │
└──────────┘
```

## Frontend Architecture

### Folder Structure

```
src/
├── components/           # Reusable components
│   ├── Header.vue
│   ├── Footer.vue
│   └── ...
├── pages/               # Route pages
│   ├── Home.vue         # Landing page
│   ├── CreateOrder.vue  # Order creation form
│   ├── Orders.vue       # Orders list
│   ├── Terms.vue        # Terms & conditions
│   └── Profile.vue      # User profile
├── services/            # API clients
│   ├── orderService.js
│   ├── authService.js
│   ├── timezones.js     # Timezone utilities
│   └── api.js
├── App.vue              # Root component
└── main.js              # Entry point
```

### State Management

Currently: **Component-based with refs/reactive**
- Simple state with `ref()` and `reactive()`
- Services communicate with backend
- No global state manager (yet)

Optional upgrade: **Pinia** for larger apps

### Services

#### `orderService.js`
```javascript
OrderService.createOrder(orderData)
OrderService.getAvailableOrders()
OrderService.updateOrder(id, data)
```

#### `timezones.js`
```javascript
getTimezoneByState(state)          // 'Jalisco' → 'CENTRO'
convertTimeToTimezone(time, zone)  // '18:00' → '17:00 día anterior'
isValidCallTime(time)              // Validar ≤ 21:00
```

## Data Flow

### Creating an Order

```
User fills form
    ↓
CreateOrder.vue validates (250 words max, timezone, time)
    ↓
POST /api/orders
    ↓
OrdersController.CreateOrder()
    ↓
OrderService.CreateOrderAsync()
    ↓
Order saved to DB with status "Pending"
    ↓
CallRetryJob.ScheduleRetries() scheduled
    ↓
Return orderId to frontend
    ↓
User proceeds to payment
```

### Retry System (3x3)

```
Order created with CallAttempts = 0
    ↓
Day 1 scheduled:
  9 AM  → ExecuteRetryAsync() → Twilio call attempt 1
  12 PM → ExecuteRetryAsync() → Twilio call attempt 2
  3 PM  → ExecuteRetryAsync() → Twilio call attempt 3
    ↓
If successful → Order.Status = "Completed"
If failed (3/3) → Schedule Day 2
    ↓
Day 2 & 3 repeat
    ↓
If all 9 fail:
  - HandleMaxRetriesAsync()
  - SMS fallback
  - Email fallback
  - Refund processed
  - Order.Status = "Failed"
```

### Payment Flow

```
Order.status = "Pending"
    ↓
Payment button clicked
    ↓
POST /api/payments/create
    ↓
MercadoPagoServiceImpl.CreatePaymentAsync()
    ↓
Payment record created in DB
    ↓
Webhook from Mercado Pago
    ↓
Payment.Status = "Completed"
Order.Status = "Assigned" (ready for messenger)
```

## Deployment Architecture

### Production Setup

```
┌────────────────────────────────────┐
│        CloudFlare (CDN)            │
└────────────────────────────────────┘
          │
┌─────────▼──────────────┐
│   Azure App Service    │
│  (Backend .NET Core)   │
└──────────┬──────────────┘
           │
┌──────────▼──────────────┐
│  Azure SQL Database     │
│   (SQL Server)          │
└─────────────────────────┘

┌─────────────────────────┐
│   Vercel / Netlify      │
│  (Frontend Vue.js)      │
└─────────────────────────┘

┌─────────────────────────┐
│  SendGrid (Email)       │
│  Twilio (Calls/SMS)     │
│  Mercado Pago (Payments)│
└─────────────────────────┘
```

## Security Architecture

### Authentication
- JWT tokens issued on login
- Tokens include user role (Buyer/Messenger)
- Expiry: 24 hours (refresh token: 30 days)

### Authorization
- Role-based access control (RBAC)
- Endpoints protected with `[Authorize(Roles="...")]`
- Buyer endpoints vs Messenger endpoints

### Data Protection
- Passwords: bcrypt hashing
- Sensitive data: encrypted in DB (Twilio SID, etc.)
- Phone numbers: validation only, not stored for anon orders
- HTTPS required for all API calls
- CORS configured for frontend domain only

## Scalability Considerations

### Database
- Indexed queries for Orders (RetryDay, CallAttempts)
- Connection pooling (30 connections default)
- Optional: Read replicas for analytics

### Backend
- Stateless design (horizontal scaling ready)
- Load balancer distributes requests
- Hangfire distributed job processing

### Frontend
- Static asset caching
- CDN for images/fonts
- Lazy loading for routes

### Mobile
- Offline support potential (future)
- Efficient API calls
- Local caching of user data

---

**Last Updated:** January 21, 2026
