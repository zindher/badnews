# 📢 Gritalo - Marketplace de Llamadas Personalizadas

**Gritalo** es una plataforma full-stack que conecta compradores con mensajeros profesionales para realizar llamadas personalizadas y memorables.

## 🎯 Características Principales

- ✅ **Llamadas Personalizadas** - Mensajes de voz grabados y entregados
- ✅ **Sistema de Reintentos 3x3** - 3 llamadas/día × 3 días (9 intentos máximo)
- ✅ **Fallback SMS/Email** - Notificaciones automáticas si falla la entrega
- ✅ **Grabación Incluida** - Descarga el video de la llamada
- ✅ **100% Anónimo** - Oculta identidad del comprador si lo desea
- ✅ **Timezone Inteligente** - Ajusta horarios por zona de México
- ✅ **Límite de Palabras** - Máximo 250 palabras (≈2 minutos)
- ✅ **Refund Automático** - Devuelve dinero si falla la entrega

## 🏗️ Stack Tecnológico

| Capa | Tecnología |
|------|-----------|
| **Backend** | .NET Core 6+ (C#), Entity Framework Core |
| **Frontend** | Vue 3 + Vite, Tailwind CSS, Axios |
| **Mobile** | Flutter (Dart) |
| **Database** | SQL Server |
| **Servicios** | Twilio, Mercado Pago, SendGrid |
| **Jobs** | Hangfire (background jobs) |

## 📁 Estructura del Proyecto

```
gritalo/
├── backend/                    # .NET Core Web API
│   ├── Models/                 # Data models
│   ├── Controllers/            # API endpoints
│   ├── Services/               # Business logic
│   ├── Validators/             # FluentValidation
│   ├── Jobs/                   # Hangfire background jobs
│   ├── Migrations/             # Entity Framework migrations
│   ├── Data/                   # DbContext
│   └── Program.cs
│
├── frontend/                   # Vue 3 + Vite
│   ├── src/
│   │   ├── components/
│   │   ├── pages/
│   │   ├── services/
│   │   └── App.vue
│   ├── package.json
│   └── vite.config.js
│
├── mobile/                     # Flutter
│   ├── lib/
│   │   ├── models/
│   │   ├── screens/
│   │   ├── services/
│   │   └── main.dart
│   └── pubspec.yaml
│
├── docs/                       # Documentation
│   ├── SETUP.md
│   ├── ARCHITECTURE.md
│   ├── API.md
│   └── DEPLOYMENT.md
│
└── .env.example
```

## 🚀 Quick Start

### Prerequisites
- Node.js 16+ (Frontend)
- .NET 6+ SDK (Backend)
- Flutter SDK (Mobile)
- SQL Server (Database)

### Backend

```bash
cd backend
dotnet restore
dotnet ef database update
dotnet run
```
→ http://localhost:5000

### Frontend

```bash
cd frontend
npm install
npm run dev
```
→ http://localhost:5173

### Mobile

```bash
cd mobile
flutter pub get
flutter run
```

## 🔑 Key Features

### Sistema 3x3 (Retry)
- 3 llamadas/día × 3 días = 9 intentos máximo
- Horarios: 9 AM, 12 PM, 3 PM
- SMS fallback si falla
- Refund automático

**Código:** `backend/Jobs/CallRetryJob.cs`

### Timezones
- 5 zonas de México
- 32 estados mapeados
- Conversión en tiempo real
- Máximo 21:00 para llamadas

**Código:** 
- `frontend/src/services/timezones.js`
- `backend/Services/TimezoneService.cs`

### Validación de Palabras
- Máximo 250 palabras
- ~2 minutos duración
- Contador visual

**Código:** 
- `frontend/src/pages/CreateOrder.vue`
- `backend/Validators/Validators.cs`

## 🔐 Security

- JWT authentication
- HTTPS required
- Input validation (FluentValidation)
- CORS configured
- No exposicion de datos anónimos

## 📊 API Endpoints

| Método | Endpoint | Descripción |
|--------|----------|-------------|
| POST | `/api/auth/register` | Registrar |
| POST | `/api/auth/login` | Login |
| POST | `/api/orders` | Crear orden |
| GET | `/api/orders/available` | Órdenes disponibles |
| PUT | `/api/orders/{id}/accept` | Aceptar orden |
| POST | `/api/calls/make-call` | Hacer llamada |

Ver `docs/API.md` para más detalles.

## 📚 Documentation

- [SETUP.md](docs/SETUP.md) - Instalación
- [ARCHITECTURE.md](docs/ARCHITECTURE.md) - Diseño
- [DEPLOYMENT.md](docs/DEPLOYMENT.md) - Deploy
- [RETRY_SYSTEM.md](docs/RETRY_SYSTEM.md) - Sistema 3x3

## 🧪 Testing

```bash
# Backend
cd backend && dotnet test

# Frontend
cd frontend && npm run test
```

## 📦 Docker

```bash
docker-compose up
```

## 🤝 Contributing

1. `git checkout -b feature/name`
2. Make changes
3. `git commit -m "feat: description"`
4. Push & create PR

## 📞 Support

- Email: support@gritalo.mx
- Issues: GitHub
- Docs: `/docs` folder

## 📄 License

MIT License

---

**Versión:** 1.0.0 | **Status:** Production Ready ✅ | **Enero 2026**
