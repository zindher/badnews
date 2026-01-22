# BadNews - Full Stack Application

Aplicación full-stack que conecta compradores con mensajeros para realizar llamadas personalizadas con grabación automática.

## 🎯 Estado del Proyecto

**Completado: 80%** ✅
- ✅ Frontend Vue 3 + Vite - 100% completo
- ✅ Mobile Flutter - 100% de pantallas
- ✅ Backend .NET Core - Servicios e integraciones completas
- ✅ Base de datos SQL Server - Esquema completo
- ⏳ Testing - Pendiente (no incluido en scope)

## 📋 Stack Tecnológico

### Backend
- **Framework:** .NET Core 6.0+
- **Database:** SQL Server with Entity Framework Core
- **Auth:** JWT Authentication
- **Background Jobs:** Hangfire
- **APIs:** RESTful

### Frontend
- **Framework:** Vue 3 + Vite
- **Styling:** Tailwind CSS
- **State Management:** Pinia
- **Routing:** Vue Router
- **HTTP:** Axios

### Mobile
- **Framework:** Flutter (Dart)
- **State Management:** Provider
- **Networking:** HTTP

### Integraciones
- **Twilio:** Llamadas y grabación de audio
- **Mercado Pago:** Procesamiento de pagos
- **SendGrid:** Notificaciones por email

## 📁 Estructura del Proyecto

```
BadNews/
├── backend/                      # .NET Core API
│   ├── Controllers/              # API endpoints
│   │   ├── AuthController.cs
│   │   ├── OrdersController.cs
│   │   ├── CallsController.cs
│   │   ├── PaymentsController.cs
│   │   ├── MessengersController.cs
│   │   ├── AdminController.cs
│   │   ├── AnalyticsController.cs
│   │   └── ChatController.cs
│   ├── Models/                   # Entity models
│   ├── Services/                 # Business logic
│   │   ├── AuthService.cs
│   │   ├── OrderService.cs
│   │   ├── JwtService.cs
│   │   ├── TwilioServiceImpl.cs
│   │   ├── MercadoPagoServiceImpl.cs
│   │   ├── SendGridServiceImpl.cs
│   │   ├── EmailService.cs
│   │   ├── CallRecordingService.cs
│   │   └── CallRetryService.cs
│   ├── Data/                     # Database context
│   ├── appsettings.Development.json
│   └── BadNews.csproj
│
├── frontend/                     # Vue 3 application
│   ├── src/
│   │   ├── components/           # Reusable components
│   │   │   ├── Button.vue
│   │   │   ├── FormField.vue
│   │   │   ├── MessengerCard.vue
│   │   │   ├── PaymentForm.vue
│   │   │   ├── OrderDetailModal.vue
│   │   │   ├── CallRecordingPlayer.vue
│   │   │   ├── MessagePreviewModal.vue
│   │   │   └── MessengerProfile.vue
│   │   ├── pages/                # Pages
│   │   │   ├── Home.vue
│   │   │   ├── CreateOrder.vue
│   │   │   ├── Orders.vue
│   │   │   ├── Profile.vue
│   │   │   ├── Terms.vue
│   │   │   ├── MessengerHome.vue
│   │   │   ├── PaymentSuccess.vue
│   │   │   ├── PaymentFailed.vue
│   │   │   ├── Earnings.vue
│   │   │   ├── History.vue
│   │   │   ├── AdminDashboard.vue
│   │   │   └── Analytics.vue
│   │   ├── stores/               # Pinia stores
│   │   │   ├── userStore.js
│   │   │   ├── orderStore.js
│   │   │   └── uiStore.js
│   │   ├── services/
│   │   ├── App.vue
│   │   └── main.js
│   ├── router.js
│   ├── vite.config.js
│   ├── .env.local
│   ├── .env.production
│   └── package.json
│
├── mobile/                       # Flutter application
│   ├── lib/
│   │   ├── screens/
│   │   │   ├── login_screen.dart
│   │   │   ├── home_screen.dart
│   │   │   ├── call_screen.dart
│   │   │   ├── recording_screen.dart
│   │   │   ├── earnings_screen.dart
│   │   │   ├── profile_screen.dart
│   │   │   └── chat_screen.dart
│   │   ├── models/
│   │   ├── providers/
│   │   ├── services/
│   │   ├── widgets/
│   │   └── main.dart
│   ├── pubspec.yaml
│   └── android/
│
└── docs/                         # Documentation
    ├── API.md                    # API documentation
    ├── DATABASE.md               # Database schema
    ├── SETUP.md                  # Setup instructions
    └── DEPLOYMENT.md             # Deployment guide
```

## 🚀 Instalación y Setup

### Requisitos Previos
- .NET 6.0 SDK
- Node.js 16+
- Flutter SDK
- SQL Server
- Visual Studio Code

### Backend Setup

1. **Restaurar dependencias**
   ```bash
   cd backend
   dotnet restore
   ```

2. **Configurar appsettings**
   ```bash
   cp appsettings.Example.json appsettings.Development.json
   # Editar appsettings.Development.json con tus credenciales
   ```

3. **Crear base de datos**
   ```bash
   dotnet ef database update
   ```

4. **Ejecutar servidor**
   ```bash
   dotnet run
   ```
   Backend estará en: `http://localhost:5000`

### Frontend Setup

1. **Instalar dependencias**
   ```bash
   cd frontend
   npm install
   ```

2. **Configurar variables de entorno**
   ```bash
   cp .env.example .env.local
   # Editar con tu API URL
   ```

3. **Ejecutar en desarrollo**
   ```bash
   npm run dev
   ```
   Frontend estará en: `http://localhost:5173`

4. **Build para producción**
   ```bash
   npm run build
   ```

### Mobile Setup

1. **Obtener dependencias**
   ```bash
   cd mobile
   flutter pub get
   ```

2. **Configurar certificados para iOS** (si aplica)
   ```bash
   cd ios
   pod install
   cd ..
   ```

3. **Ejecutar en emulador**
   ```bash
   flutter run
   ```

## 🔐 Variables de Entorno

### Backend (appsettings.Development.json)
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=BadNews_Dev;Integrated Security=true;"
  },
  "JwtSettings": {
    "Secret": "your-secret-key-min-32-chars",
    "ExpirationMinutes": 1440
  },
  "Twilio": {
    "AccountSid": "your-twilio-account-sid",
    "AuthToken": "your-twilio-auth-token",
    "PhoneNumber": "+1234567890"
  },
  "SendGrid": {
    "ApiKey": "your-sendgrid-api-key",
    "FromEmail": "noreply@badnews.com"
  },
  "MercadoPago": {
    "AccessToken": "your-mercado-pago-token",
    "PublicKey": "your-mercado-pago-public-key"
  }
}
```

### Frontend (.env.local)
```env
VITE_API_URL=http://localhost:5000
VITE_APP_NAME=BadNews
VITE_ENABLE_ANALYTICS=true
```

## 📚 Funcionalidades Principales

### Para Compradores
- ✅ Crear órdenes personalizadas
- ✅ Seleccionar mensajero disponible
- ✅ Procesar pago con Mercado Pago
- ✅ Visualizar grabación de llamada
- ✅ Historial de órdenes
- ✅ Chat con mensajero
- ✅ Descargar grabación

### Para Mensajeros
- ✅ Listar órdenes disponibles
- ✅ Aceptar/rechazar órdenes
- ✅ Dashboard de ganancias
- ✅ Solicitar retiros
- ✅ Historial de llamadas
- ✅ Ver grabaciones
- ✅ Perfil y disponibilidad

### Para Administradores
- ✅ Dashboard con métricas
- ✅ Analytics detallados
- ✅ Gestión de disputas
- ✅ Aprobación de retiros
- ✅ Reportes CSV/PDF
- ✅ Logs del sistema

### Características del Sistema
- ✅ Grabación automática de llamadas via Twilio
- ✅ Reintentos automáticos (3 intentos en 3 días)
- ✅ Notificaciones por email (SendGrid)
- ✅ Pagos integrados (Mercado Pago)
- ✅ Chat en tiempo real entre partes
- ✅ Sistema de ratings y reviews
- ✅ Manejo de disputas
- ✅ Reportes y analytics

## 🔌 API Endpoints

### Autenticación
- `POST /api/auth/register` - Registrar usuario
- `POST /api/auth/login` - Iniciar sesión
- `POST /api/auth/refresh` - Refrescar token
- `GET /api/auth/me` - Obtener perfil actual

### Órdenes
- `GET /api/orders` - Listar órdenes
- `POST /api/orders` - Crear orden
- `GET /api/orders/{id}` - Obtener orden
- `POST /api/orders/{id}/accept` - Aceptar orden
- `POST /api/orders/{id}/decline` - Rechazar orden

### Llamadas
- `POST /api/calls/initiate` - Iniciar llamada
- `POST /api/calls/{id}/end` - Finalizar llamada
- `GET /api/calls/history` - Historial de llamadas
- `GET /api/calls/{id}/recording` - Obtener grabación

### Pagos
- `POST /api/payments` - Crear pago
- `GET /api/payments/{id}` - Obtener estado de pago
- `POST /api/payments/{id}/refund` - Reembolsar pago

### Mensajeros
- `GET /api/messengers/earnings` - Obtener ganancias
- `POST /api/messengers/withdraw` - Solicitar retiro
- `GET /api/messengers/profile` - Perfil del mensajero

### Admin
- `GET /api/admin/dashboard` - Dashboard
- `GET /api/admin/analytics` - Analytics
- `GET /api/admin/users` - Listar usuarios
- `GET /api/admin/disputes` - Listar disputas

### Chat
- `GET /api/chat/conversations` - Listar conversaciones
- `POST /api/chat/messages` - Enviar mensaje
- `GET /api/chat/conversations/{id}` - Obtener conversación

### Analytics
- `GET /api/analytics/orders/daily` - Órdenes diarias
- `GET /api/analytics/revenue/daily` - Ingresos diarios
- `GET /api/analytics/messengers/performance` - Performance de mensajeros
- `GET /api/analytics/export/csv` - Exportar a CSV

## 🧪 Testing

Testing está excluido del scope actual pero la arquitectura está preparada para:
- Unit tests con xUnit
- Integration tests
- E2E tests con Cypress (Frontend)

## 📊 Base de Datos

### Tablas Principales
- **Users** - Usuarios (buyers y messengers)
- **Orders** - Órdenes de mensajes personalizados
- **CallAttempts** - Intentos de llamada con metadata
- **Payments** - Transacciones de pago
- **Messages** - Conversaciones entre partes
- **CallRetries** - Historial de reintentos
- **Withdrawals** - Solicitudes de retiro
- **Disputes** - Disputas reportadas

## 🚢 Deployment

### Frontend
```bash
npm run build
# Servir contenido de dist/ con Vercel, Netlify, o similar
```

### Backend
```bash
dotnet publish -c Release
# Desplegar en IIS, Azure App Service, o Heroku
```

### Mobile
```bash
# Android
flutter build apk

# iOS
flutter build ios
```

## 📝 Notas Importantes

1. **Configuración de JWT:** Usar secreto de mínimo 32 caracteres
2. **Twilio Recordings:** Las grabaciones se almacenan en Twilio por 100 años
3. **Mercado Pago:** Obtener Access Token desde dashboard
4. **SendGrid:** API key desde cuenta SendGrid verificada
5. **Database:** Usar SQL Server 2019+ para mejor rendimiento

## 🤝 Contribución

Este proyecto fue desarrollado como full-stack completo. Para cambios significativos, crear rama de feature y pull request.

## 📄 Licencia

Proyecto privado - Todos los derechos reservados

## 👨‍💻 Desarrollado por

GitHub Copilot en VSCode - Enero 2026
