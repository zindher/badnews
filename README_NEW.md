# BadNews - Plataforma de Llamadas Personalizadas

![BadNews](https://img.shields.io/badge/Status-Development-yellow)
![License](https://img.shields.io/badge/License-MIT-blue)
![Progress](https://img.shields.io/badge/Progress-65%25-brightgreen)

## 🎯 Descripción General

BadNews es una plataforma de tres capas que conecta a **compradores** con **mensajeros** para realizar llamadas personalizadas grabadas. Los usuarios pueden pagar para que alguien llame a un receptor y entregue un mensaje personalizado.

**Características Principales:**
- 📞 Llamadas personalizadas grabadas
- 💰 Pagos integrados con Mercado Pago
- 🔄 Sistema inteligente de reintentos (3 llamadas/día × 3 días)
- 🎙️ Grabación automática de llamadas
- 📧 Notificaciones por email y SMS
- 🔐 Autenticación segura con JWT
- ⭐ Sistema de calificaciones para mensajeros

## 🛠 Stack Tecnológico Confirmado

| Capa | Tecnología |
|------|-----------|
| Backend | .NET 8 Core Web API |
| Frontend | Vue 3 + Vite |
| Mobile | Flutter (Dart) |
| Database | SQL Server |
| Auth | JWT Tokens |
| Jobs | Hangfire |
| Servicios | Twilio, Mercado Pago, SendGrid |
| DevOps | Docker, GitHub Actions |

## 🚀 Inicio Rápido

### Opción 1: Docker (Recomendado)
```bash
cp .env.example .env
docker-compose up -d
```

### Opción 2: Manual
```bash
# Backend
cd backend && dotnet run

# Frontend  
cd frontend && npm install && npm run dev

# Mobile
cd mobile && flutter run
```

## 📚 Documentación Completa

- [LOCAL_DEVELOPMENT.md](LOCAL_DEVELOPMENT.md) - Setup detallado
- [API_TESTING_GUIDE.md](API_TESTING_GUIDE.md) - Ejemplos de API
- [TWILIO_IMPLEMENTATION.md](TWILIO_IMPLEMENTATION.md) - Integración Twilio
- [DEPLOYMENT.md](DEPLOYMENT.md) - Deployment a producción
- [PROGRESS_REPORT.md](PROGRESS_REPORT.md) - Estado del proyecto

## 📊 Estado Actual

**Progreso: 65%** (Phase 2 - Implementation)

- ✅ Backend scaffolding (100%)
- ✅ Frontend scaffolding (100%)
- ✅ Mobile scaffolding (100%)
- ✅ JWT Authentication (100%)
- ✅ Error Handling & Validation (100%)
- 🔄 Twilio Integration (30%)
- 🔄 Mercado Pago (20%)
- ⏳ SendGrid (10%)
- ⏳ Testing (0%)
- ⏳ Deployment (0%)

## 📡 API Endpoints

```bash
# Auth
POST   /api/auth/register
POST   /api/auth/login
GET    /api/auth/profile

# Órdenes
POST   /api/orders
GET    /api/orders/available
POST   /api/orders/{id}/accept
POST   /api/orders/{id}/rate

# Messengers
GET    /api/messengers/{id}
PUT    /api/messengers/{id}/availability

# Pagos
POST   /api/payments
GET    /api/payments/{id}
```

Ver [API_TESTING_GUIDE.md](API_TESTING_GUIDE.md) para ejemplos completos.

## 🧪 Testing Rápido

```bash
# Registrarse
curl -X POST http://localhost:5000/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "email":"test@example.com",
    "password":"SecurePass123!",
    "firstName":"Juan",
    "lastName":"Pérez",
    "phoneNumber":"+5215551234567",
    "role":"Buyer"
  }'
```

## 📦 Estructura del Proyecto

```
badnews/
├── backend/              # .NET 8 Web API
│   ├── Models/
│   ├── Controllers/      (5 controllers, 17 endpoints)
│   ├── Services/         (6 servicios)
│   ├── DTOs/            (20+ DTOs)
│   ├── Validators/      (7 validadores)
│   ├── Middleware/      (Error handling)
│   ├── Jobs/            (Hangfire retry job)
│   └── Program.cs       (DI & configuration)
│
├── frontend/             # Vue 3 + Vite
│   ├── src/
│   │   ├── components/
│   │   ├── pages/       (4 pages)
│   │   ├── services/    (API client)
│   │   └── stores/      (Pinia)
│   └── vite.config.js
│
├── mobile/               # Flutter
│   ├── lib/
│   │   ├── models/
│   │   ├── screens/     (2 screens)
│   │   ├── services/    (API, Recording)
│   │   └── providers/   (State management)
│   └── pubspec.yaml
│
├── docker-compose.yml
├── .env.example
├── .github/workflows/    (CI/CD)
└── docs/                 (12+ guías)
```

## 🔐 Configuración

Crear `.env`:
```env
DB_CONNECTION_STRING=Server=localhost;Database=BadNews;User Id=sa;Password=YourPassword123!;
JWT_SECRET=your-key-at-least-32-characters-long
TWILIO_ACCOUNT_SID=AC...
TWILIO_AUTH_TOKEN=...
MERCADO_PAGO_ACCESS_TOKEN=...
SENDGRID_API_KEY=...
```

## 📈 Estadísticas del Proyecto

- **Total Files:** 55
- **Lines of Code:** 7,500+
- **Controllers:** 5
- **API Endpoints:** 17
- **Database Models:** 5
- **Tests:** Pending
- **Documentation:** 12+ pages

## 🔄 Próximos Pasos

1. **Twilio Integration** (2-3 días)
   - Implementar TwilioRestClient
   - Generar TwiML dinámico
   - Testing con números de prueba

2. **Mercado Pago** (2 días)
   - Crear órdenes de pago
   - Procesar pagos
   - Webhooks

3. **SendGrid + SMS** (1-2 días)
   - Email notifications
   - SMS fallback

4. **Testing** (3-4 días)
   - Unit tests
   - Integration tests
   - E2E tests

5. **Deployment** (2-3 días)
   - Azure/AWS setup
   - Production deployment

## 📞 Soporte

- 📧 Email: support@badnews.mx
- 📚 Docs: [Ver documentación completa](/docs)
- 🐛 Issues: GitHub Issues

---

**Version:** 0.65.0
**Última actualización:** Enero 2024
**Licencia:** MIT
