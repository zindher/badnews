# BadNews Project Quick Start Guide

## Paso 1: Clonar y Preparar Repositorio

```bash
git clone https://github.com/yourusername/badnews.git
cd badnews
```

## Paso 2: Backend Setup (.NET Core)

### 2.1 Instalar SQL Server
- Descargar desde: https://www.microsoft.com/en-us/sql-server/sql-server-downloads
- Crear base de datos `BadNews_Dev`

### 2.2 Configurar Backend

```bash
cd backend

# Restaurar paquetes NuGet
dotnet restore

# Crear appsettings.Development.json
cat > appsettings.Development.json << EOF
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft.AspNetCore": "Debug"
    }
  },
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=BadNews_Dev;Integrated Security=true;"
  },
  "JwtSettings": {
    "Secret": "your-super-secret-key-min-32-chars!!!!",
    "ExpirationMinutes": 1440,
    "RefreshTokenExpirationDays": 7
  },
  "Twilio": {
    "AccountSid": "AC_YOUR_ACCOUNT_SID",
    "AuthToken": "your_auth_token",
    "PhoneNumber": "+1234567890"
  },
  "SendGrid": {
    "ApiKey": "SG_YOUR_API_KEY",
    "FromEmail": "noreply@badnews.com",
    "FromName": "BadNews"
  },
  "MercadoPago": {
    "AccessToken": "APP_USR_YOUR_TOKEN",
    "PublicKey": "your_public_key"
  },
  "Hangfire": {
    "ConnectionString": "Server=.;Database=BadNews_Hangfire_Dev;Integrated Security=true;"
  },
  "Features": {
    "CallRetryEnabled": true,
    "MaxRetries": 3,
    "RetryIntervalDays": 1
  }
}
EOF

# Ejecutar migraciones de BD
dotnet ef database update

# Ejecutar servidor (puerto 5000)
dotnet run

# Backend disponible en: http://localhost:5000
```

### 2.3 Obtener Credenciales de APIs

**Twilio:**
1. Crear cuenta en https://www.twilio.com
2. Copiar Account SID y Auth Token
3. Generar número telefónico
4. Configurar webhook para grabaciones

**SendGrid:**
1. Crear cuenta en https://sendgrid.com
2. Crear API Key
3. Verificar dominio para envío

**Mercado Pago:**
1. Crear cuenta en https://www.mercadopago.com.ar
2. Acceder a Configuración > Credenciales
3. Copiar Access Token

## Paso 3: Frontend Setup (Vue 3)

```bash
cd ../frontend

# Instalar dependencias
npm install

# Crear .env.local
cat > .env.local << EOF
VITE_API_URL=http://localhost:5000
VITE_APP_NAME=BadNews
VITE_ENABLE_ANALYTICS=true
EOF

# Ejecutar en desarrollo (puerto 5173)
npm run dev

# Frontend disponible en: http://localhost:5173
```

## Paso 4: Mobile Setup (Flutter)

```bash
cd ../mobile

# Instalar dependencias Flutter
flutter pub get

# Ejecutar en emulador
flutter run

# O construir APK para Android
flutter build apk

# O construir para iOS
flutter build ios
```

## Paso 5: Verificar Instalación

### Backend Health Check
```bash
curl http://localhost:5000/api/auth/health
```

### Frontend
Abrir http://localhost:5173 en navegador

### Probar Endpoints

```bash
# Registrar usuario
curl -X POST http://localhost:5000/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "email": "buyer@example.com",
    "password": "Test@1234",
    "name": "John Buyer",
    "userType": "buyer"
  }'

# Login
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "buyer@example.com",
    "password": "Test@1234"
  }'
```

## 🔧 Troubleshooting

### Backend no inicia
- Verificar SQL Server está corriendo: `sqlcmd -S . -E`
- Verificar ConnectionString en appsettings.Development.json
- Limpiar y restaurar: `dotnet clean && dotnet restore`

### Frontend no compila
- Limpiar node_modules: `rm -rf node_modules && npm install`
- Verificar versión Node.js: `node --version` (debe ser 16+)
- Verificar VITE_API_URL en .env.local

### Flutter build falla
- Verificar Flutter version: `flutter --version`
- Limpiar build: `flutter clean`
- Obtener dependencias: `flutter pub get`

### Twilio/SendGrid no funciona
- Verificar credenciales en appsettings.Development.json
- Comprobar límites de API (rate limiting)
- Ver logs en consola del backend

## 📚 Próximos Pasos

1. **Implementar Tests** - Unit tests y integration tests
2. **Configurar CI/CD** - GitHub Actions, Azure DevOps
3. **Setup de Producción** - Certificados SSL, DNS
4. **Monitoring** - Application Insights, ELK Stack
5. **Optimizaciones** - Caching, CDN, Database indexing

## 📞 Soporte

Para reportar problemas o preguntas:
1. Revisar logs del backend: `dotnet run --verbose`
2. Revisar console del frontend: F12 en navegador
3. Verificar documentación en `/docs`

## ✅ Checklist de Setup

- [ ] SQL Server instalado y corriendo
- [ ] Backend clonado y configurado
- [ ] appsettings.Development.json completado
- [ ] Base de datos creada (ef database update)
- [ ] Backend ejecutándose en puerto 5000
- [ ] Frontend clonado y .env.local configurado
- [ ] npm dependencies instaladas
- [ ] Frontend ejecutándose en puerto 5173
- [ ] Flutter SDK instalado
- [ ] Emulador de Android/iOS configurado
- [ ] Credenciales de APIs completadas
- [ ] Primeros tests funcionales completados

¡Listo para desarrollar! 🚀
