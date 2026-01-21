# Setup & Installation Guide

## Requirements

- **Node.js** 16+ (Frontend)
- **.NET SDK** 6.0+ (Backend)
- **Flutter SDK** 3.0+ (Mobile)
- **SQL Server** 2019+ (Database)
- **Git** 2.30+

## Backend Setup

### 1. Database Configuration

```bash
cd backend

# Create database
dotnet ef database update

# Or manually:
# Create SQL Server database named 'GritaloDb'
```

### 2. Configuration Files

Create `appsettings.Development.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=GritaloDb;Integrated Security=true;"
  },
  "Jwt": {
    "Key": "your-secret-key-min-32-chars",
    "Issuer": "gritalo.mx",
    "Audience": "gritalo-app"
  }
}
```

### 3. User Secrets (Sensitive Data)

```bash
dotnet user-secrets init

# Twilio
dotnet user-secrets set "Twilio:AccountSid" "your-sid"
dotnet user-secrets set "Twilio:AuthToken" "your-token"
dotnet user-secrets set "Twilio:PhoneNumber" "+1234567890"

# SendGrid
dotnet user-secrets set "SendGrid:ApiKey" "your-key"

# Mercado Pago
dotnet user-secrets set "MercadoPago:AccessToken" "your-token"
```

### 4. Run Backend

```bash
dotnet run

# Or with watch mode
dotnet watch run
```

API runs on: **http://localhost:5000**

## Frontend Setup

### 1. Install Dependencies

```bash
cd frontend
npm install
```

### 2. Environment Variables

Create `.env.local`:
```env
VITE_API_BASE_URL=http://localhost:5000/api
VITE_APP_NAME=Gritalo
```

### 3. Run Frontend

```bash
npm run dev
```

Frontend runs on: **http://localhost:5173**

### 4. Build for Production

```bash
npm run build
npm run preview
```

## Mobile Setup

### 1. Get Flutter

```bash
# Install Flutter from https://flutter.dev
flutter doctor

# Verify setup
flutter --version
```

### 2. Get Dependencies

```bash
cd mobile
flutter pub get
```

### 3. Configure API URL

Edit `lib/services/api_service.dart`:
```dart
static const String baseUrl = 'http://localhost:5000/api';
```

### 4. Run on Device/Emulator

```bash
# iOS
flutter run -d iPhone

# Android
flutter run -d Android
```

## Docker Setup (All-in-One)

### 1. Build Images

```bash
docker-compose build
```

### 2. Start Services

```bash
docker-compose up -d
```

Services:
- Backend: http://localhost:5000
- Frontend: http://localhost:3000
- SQL Server: localhost:1433

### 3. Stop Services

```bash
docker-compose down
```

## Testing

### Backend Unit Tests

```bash
cd backend
dotnet test
```

### Frontend Tests

```bash
cd frontend
npm run test
```

## Troubleshooting

### "Connection to database failed"
- Ensure SQL Server is running
- Check connection string in `appsettings.json`
- Verify database exists

### "Cannot resolve npm packages"
```bash
cd frontend
rm -rf node_modules package-lock.json
npm install
```

### "Flutter doctor errors"
```bash
flutter doctor -v
flutter doctor --android-licenses
```

### Port Already in Use
```bash
# Backend (5000)
netstat -ano | findstr :5000

# Frontend (5173)
npm run dev -- --port 5174
```

## Environment Variables Reference

### Backend
| Variable | Example | Required |
|----------|---------|----------|
| `Twilio:AccountSid` | ACxxxxxxxxx | Yes |
| `Twilio:AuthToken` | xxxxxxxx | Yes |
| `SendGrid:ApiKey` | SG.xxxxxxx | Yes |
| `MercadoPago:AccessToken` | APP_xxx | Yes |

### Frontend
| Variable | Default |
|----------|---------|
| `VITE_API_BASE_URL` | http://localhost:5000/api |
| `VITE_APP_NAME` | Gritalo |

## Next Steps

- Read [ARCHITECTURE.md](ARCHITECTURE.md) to understand system design
- Check [API.md](API.md) for endpoint documentation
- See [DEPLOYMENT.md](DEPLOYMENT.md) for production setup
