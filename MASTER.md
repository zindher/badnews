# 📢 BADNEWS - DOCUMENTACIÓN MAESTRA

**Última actualización:** 21 de Enero de 2026  
**Estado del Proyecto:** 🟢 En Desarrollo (75% Frontend, 100% BD)

---

## 📋 TABLA DE CONTENIDOS

1. [Descripción del Proyecto](#descripción-del-proyecto)
2. [Stack Tecnológico](#stack-tecnológico)
3. [Estructura del Proyecto](#estructura-del-proyecto)
4. [Setup Rápido](#setup-rápido)
5. [Base de Datos](#base-de-datos)
6. [API Endpoints](#api-endpoints)
7. [Google OAuth](#google-oauth)
8. [Términos y Condiciones](#términos-y-condiciones)
9. [Autenticación](#autenticación)
10. [Seguridad](#seguridad)
11. [Troubleshooting](#troubleshooting)

---

## 🎯 Descripción del Proyecto

**BadNews** es una plataforma full-stack que conecta:
- **Compradores (Buyers)** - Quieren enviar llamadas personalizadas
- **Mensajeros (Messengers)** - Ganan dinero entregando llamadas
- **Administradores (Admins)** - Moderan la plataforma

### Características Principales
✅ Llamadas personalizadas grabadas  
✅ Sistema 3x3 (3 intentos/día × 3 días = 9 máximo)  
✅ Pagos con Mercado Pago  
✅ Timezone inteligente (zonas de México)  
✅ 100% anónimo si lo desea  
✅ Sistema de earnings y retiros  
✅ Grabación de llamadas incluida  
✅ Google OAuth + Email/Password  

---

## 🏗️ Stack Tecnológico

| Componente | Tecnología |
|-----------|-----------|
| Backend | .NET Core 6.0+ (C#) |
| Frontend | Vue 3 + Vite + Pinia |
| Mobile | Flutter (Dart) |
| Database | SQL Server 2022 RTM-GDR |
| Auth | JWT + Google OAuth 2.0 |
| Pagos | Mercado Pago API |
| Email | SendGrid |
| Llamadas | Twilio |
| Jobs | Hangfire |

---

## 📁 Estructura del Proyecto

```
BadNews/
├── backend/                          # .NET Core API
│   ├── Models/                       # 9 entidades (User, Order, etc)
│   ├── Controllers/                  # Auth, Orders, Payments, etc
│   ├── Services/                     # Business logic
│   │   ├── AuthService.cs
│   │   ├── GoogleOAuthService.cs    # ✅ Google OAuth
│   │   ├── TwilioServiceImpl.cs      # Llamadas
│   │   ├── MercadoPagoServiceImpl.cs # Pagos
│   │   └── ...
│   ├── Data/
│   │   └── BadNewsDbContext.cs      # Entity Framework
│   ├── Migrations/                   # 3 migrations SQL
│   ├── Program.cs                    # Startup config
│   └── appsettings.json
│
├── frontend/                         # Vue 3 SPA
│   ├── src/
│   │   ├── components/               # 5 componentes
│   │   ├── pages/                    # 6+ páginas
│   │   ├── services/                 # 4 servicios API
│   │   │   ├── apiClient.js
│   │   │   ├── authService.js
│   │   │   ├── orderApiService.js
│   │   │   ├── transactionService.js
│   │   │   └── googleAuthService.js  # ✅ Google OAuth
│   │   ├── stores/                   # Pinia stores
│   │   │   ├── userStore.js
│   │   │   ├── orderStore.js
│   │   │   └── uiStore.js
│   │   ├── router.js                 # ✅ Auth Guards
│   │   └── App.vue
│   ├── package.json
│   ├── vite.config.js
│   └── .env.local
│
├── mobile/                           # Flutter
│   ├── lib/
│   │   ├── models/
│   │   ├── screens/
│   │   └── main.dart
│   └── pubspec.yaml
│
├── database/                         # 📊 SQL CONSOLIDADO
│   └── COMPLETE_DATABASE.sql         # ✅ TODO el SQL aquí
│
├── docs/                             # 📚 Documentación
│   └── [DEPRECATED - ver MASTER.md]
│
└── MASTER.md                         # 📌 TÚ ESTÁS AQUÍ
```

---

## 🚀 Setup Rápido

### Requisitos Previos
- Node.js 16+
- .NET 6+ SDK
- SQL Server 2019+
- Flutter SDK (para mobile)

### 1️⃣ Clonar y Descargar

```bash
git clone <repo>
cd BadNews
```

### 2️⃣ Backend Setup

```bash
cd backend

# Restaurar dependencias
dotnet restore

# Ejecutar migrations a BD
dotnet ef database update

# Iniciar servidor
dotnet run
```

**Backend disponible en:** `http://localhost:5000`

### 3️⃣ Frontend Setup

```bash
cd frontend

# Instalar dependencias
npm install

# Configurar variables de entorno
# Editar .env.local y agregar VITE_GOOGLE_CLIENT_ID

# Iniciar dev server
npm run dev
```

**Frontend disponible en:** `http://localhost:5173`

### 4️⃣ Variables de Entorno

#### Backend (`backend/appsettings.json`)
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=BadNews;Trusted_Connection=true;"
  },
  "Jwt": {
    "Secret": "tu-clave-secreta-jwt-muy-larga-y-segura",
    "Issuer": "badnews-api",
    "Audience": "badnews-app"
  },
  "Twilio": {
    "AccountSid": "tu_account_sid",
    "AuthToken": "tu_auth_token",
    "PhoneNumber": "+1234567890"
  },
  "MercadoPago": {
    "AccessToken": "tu_access_token"
  },
  "SendGrid": {
    "ApiKey": "tu_api_key",
    "FromEmail": "noreply@badnews.com"
  }
}
```

#### Frontend (`frontend/.env.local`)
```env
VITE_API_URL=http://localhost:5000
VITE_APP_NAME=BadNews
VITE_GOOGLE_CLIENT_ID=tu_google_client_id
```

---

## 📊 BASE DE DATOS

### Tablas Principales (10 total)

| Tabla | Propósito | Registros |
|-------|-----------|-----------|
| **Users** | Cuentas de usuario | Buyers, Messengers, Admins |
| **Orders** | Encargos de llamadas | Estado, mensajero asignado |
| **Messengers** | Perfil de mensajero | Rating, earnings |
| **Payments** | Transacciones | Mercado Pago |
| **Withdrawals** | Retiros de dinero | Solicitudes de pago |
| **CallAttempts** | Intentos de llamada | 3x3 system |
| **CallRetry** | Control de reintentos | Próximo intento |
| **Messages** | Chat entre usuarios | Mensajes privados |
| **Disputes** | Conflictos | Resolución |
| **EFMigrationsHistory** | Control de versión | Generado automáticamente |

### Campos Importantes

**Users:**
- Id (GUID)
- Email (único)
- PasswordHash
- GoogleId, GoogleEmail, IsGoogleLinked (✅ Google OAuth)
- Role (Buyer, Messenger, Admin)
- FirstName, LastName
- CreatedAt, UpdatedAt

**Orders:**
- Id (GUID)
- BuyerId, MessengerId
- RecipientName, RecipientPhone, RecipientEmail
- Message (hasta 250 palabras)
- Status (Pending, Assigned, InProgress, Completed, Cancelled)
- PaymentStatus (Pending, Completed, Refunded)
- IsAnonymous
- Price (DECIMAL)
- PreferredCallTime
- RecipientTimezone

### SQL Consolidado

✅ **Archivo:** `database/COMPLETE_DATABASE.sql`

Contiene:
- Schema de todas las tablas
- Índices de rendimiento
- Foreign keys
- Constraints
- Inserts de prueba (opcional)

---

## 🔌 API Endpoints

### Authentication

```
POST   /api/auth/login                 # Email + Password
POST   /api/auth/register              # Crear cuenta
POST   /api/auth/google-login          # Google OAuth
POST   /api/auth/link-google           # Vincular Google
POST   /api/auth/unlink-google         # Desvinc ular Google
GET    /api/users/me                   # Perfil actual
PUT    /api/users/me                   # Actualizar perfil
PUT    /api/users/me/password          # Cambiar contraseña
```

### Orders

```
POST   /api/orders                     # ✅ Crear (requiere auth)
GET    /api/orders                     # Listar (requiere auth)
GET    /api/orders/{id}                # Detalle (requiere auth)
GET    /api/orders/my-orders           # Mis órdenes (requiere auth)
GET    /api/orders/available           # Órdenes disponibles (Messenger)
PUT    /api/orders/{id}/accept         # Aceptar orden (Messenger)
PUT    /api/orders/{id}/cancel         # Cancelar orden
```

### Payments

```
POST   /api/payments                   # Crear pago
GET    /api/payments/{id}              # Estado de pago
GET    /api/payments                   # Mis pagos
POST   /api/payments/{id}/refund       # Reembolso
GET    /api/payments/methods           # Métodos de pago
```

### Earnings

```
GET    /api/messengers/me/earnings     # Ganancias totales
GET    /api/messengers/me/earnings/history # Historial
POST   /api/withdrawals                # Solicitar retiro
GET    /api/withdrawals                # Mis retiros
```

---

## 🔐 Google OAuth

### Configuración Requerida

1. **Obtén Google Client ID:**
   - Ve a https://console.cloud.google.com
   - Crea OAuth 2.0 Client ID (Web application)
   - URIs autorizadas: `http://localhost:5173`, `http://localhost:5000`

2. **Configura .env.local:**
   ```env
   VITE_GOOGLE_CLIENT_ID=tu_client_id_aqui
   ```

3. **Endpoints Google OAuth:**
   ```
   POST /api/auth/google-login    # Login con Google
   POST /api/auth/link-google     # Vincular a cuenta existente
   POST /api/auth/unlink-google   # Desvinc ular
   ```

### Campos BD para Google

Tabla `Users` contiene:
- `GoogleId` - ID único de Google
- `GoogleEmail` - Email de Google
- `GoogleProfilePictureUrl` - Foto de perfil
- `IsGoogleLinked` - Indica vinculación

---

## � Términos y Condiciones

### Sistema 3x3 y Política de Reembolsos

BadNews utiliza un sistema de reintento automático:

**Ciclo de Reintento:**
- **Duración:** 3 días consecutivos
- **Intentos:** 3 llamadas por día (máximo)
- **Total:** Hasta 9 intentos de contacto
- **Horarios:** Respeta zonas horarias configuradas

**Fallback a SMS:**
Si después de 9 intentos de llamada no hay contacto:
- Sistema automático envía SMS con el mensaje
- SMS es método final de entrega
- Se considera ciclo completado exitosamente

### Política de Reembolsos

**BadNews completa su servicio cuando:**
✓ Realiza los 3 intentos/día × 3 días  
✓ Respeta horarios y zonas horarias  
✓ Intenta contacto vía SMS como fallback  

**NO se otorgan reembolsos por:**
✗ Número incorrecto o inactivo (responsabilidad del comprador)  
✗ Número bloqueado o no disponible  
✗ Dispositivo apagado o sin cobertura  
✗ Fallos técnicos de operadora (ajeno a BadNews)  
✗ Destinatario rechaza llamada intencionalmente  
✗ Mensaje entregado por SMS exitosamente  

**Punto crítico:**
BadNews actúa como intermediario de entrega. Una vez completado el ciclo 3x3 + SMS, BadNews ha cumplido su contrato. La responsabilidad de números válidos es del comprador.

### Implementación en Frontend

**Componentes:**
- `TermsAndConditionsModal.vue` - Modal con T&C completos
- `TermsAndConditions.vue` - Página standalone en `/terms-conditions`

**Flujo de Registro:**
1. Usuario completa formulario
2. Hace clic en "Registrarse"
3. Modal aparece (BLOQUEANTE - debe leer T&C)
4. Scrollea hasta 90% para habilitar checkbox
5. Marca "Acepto Términos y Condiciones"
6. Confirma - se registra con timestamp

**Campo en Base de Datos:**
- `TermsAcceptedAt` - Fecha/hora de aceptación
- `TermsAcceptedVersion` - Versión de T&C aceptada (ej: 1.0)

### Backend - Validación

En `AuthService.RegisterAsync()`:
```csharp
// Validar T&C aceptados
if (!request.TermsAcceptedAt.HasValue)
    throw new InvalidOperationException("Terms and Conditions must be accepted");

user.TermsAcceptedAt = request.TermsAcceptedAt;
user.TermsAcceptedVersion = "1.0";
```

---

## �🔐 Autenticación y Seguridad

### 4 Capas de Protección

**Capa 1: Router Guard (Frontend)**
- Verifica `meta.requiresAuth` antes de navegar
- Redirige a `/login` si no está autenticado

**Capa 2: Componente (Frontend)**
- Verifica `userStore.isAuthenticated`
- Muestra alerta si no está autenticado

**Capa 3: API Client (Frontend)**
- Agrega JWT token en header `Authorization`
- Intercepta errores 401 → redirige a login

**Capa 4: Backend (C#)**
- `[Authorize]` valida JWT
- `[Authorize(Roles = "Buyer")]` valida rol

### Rutas Protegidas

```
✅ /orders              - Requiere autenticación
✅ /orders/new          - Requiere autenticación
✅ /orders/:id          - Requiere autenticación
✅ /profile             - Requiere autenticación
✅ /messenger/home      - Requiere autenticación
✅ /admin/dashboard     - Requiere autenticación + rol Admin

🌍 /                    - Público
🌍 /login               - Público
🌍 /terms               - Público
```

### JWT Token

```javascript
{
  "sub": "user-guid",
  "email": "user@example.com",
  "role": "Buyer",
  "exp": 1234567890,
  "iat": 1234567890
}
```

Token almacenado en `localStorage` como `auth_token`

---

## 🛡️ Validaciones y Seguridad

### Validaciones Frontend
- Email válido (regex)
- Contraseña mínimo 6 caracteres
- Campos requeridos

### Validaciones Backend
- FluentValidation en DTOs
- Modelo State validation
- Boundary checks (250 palabras máximo)

### CORS
- Configurado para `http://localhost:5173`
- En producción: especificar dominio

### SQL Injection Prevention
- Parámetros en todas las queries
- Entity Framework Core previene ataques

---

## 🔧 Troubleshooting

### Frontend no carga
```bash
cd frontend
rm -r node_modules .vite
npm install
npm run dev
```

### Backend error de conexión BD
```bash
cd backend
dotnet ef database update  # Ejecutar migrations
dotnet run
```

### "No matching export" error
- Verifica que los servicios exportan correctamente
- Recarga el servidor (Ctrl+C, npm run dev)

### 401 Unauthorized en API
- Verifica que JWT token está en localStorage
- Comprueba que token no expiró
- Verifica que rol es correcto

### Google OAuth no funciona
- Verifica `VITE_GOOGLE_CLIENT_ID` en .env.local
- Recarga la página (Ctrl+R)
- Abre DevTools Console para ver errores

---

## 📈 Progreso del Proyecto

| Componente | Status | % |
|-----------|--------|---|
| Database Schema | ✅ COMPLETE | 100% |
| Backend API | ✅ COMPLETE | 100% |
| Frontend Services | ✅ COMPLETE | 100% |
| Frontend Pages | 🔄 75% | 75% |
| Mobile App | 🔄 20% | 20% |
| Testing | ⏳ 5% | 5% |
| Deployment | ⏳ 0% | 0% |

---

## 📱 Tecnologías Versiones

```
.NET Core: 6.0+
Vue.js: 3.3+
Vite: 5.4+
Pinia: 2.1+
Axios: 1.6+
Flutter: 3.1+
SQL Server: 2022 RTM-GDR
Node.js: 18+
```

---

## 👥 Roles y Permisos

### Buyer (Comprador)
- Crear órdenes
- Ver sus órdenes
- Pagar con Mercado Pago
- Solicitar reembolsos
- Chat con mensajero

### Messenger (Mensajero)
- Ver órdenes disponibles
- Aceptar órdenes
- Hacer llamadas
- Ver ganancias
- Solicitar retiros
- Chat con comprador

### Admin
- Ver todas las órdenes
- Ver analytics
- Resolver disputas
- Moderar contenido
- Ver reportes

---

## 🎨 Arquitectura CSS Global

### Sistema de Variables CSS (Master Theme)
Ubicación: `frontend/src/styles/variables.css`

El proyecto utiliza **CSS variables globales** para mantener consistencia de diseño y facilitar cambios de tema. Todos los colores, tipografías y espaciados se definen en un único archivo.

#### Color System
```css
/* Colores Primarios */
--primary-color: #5B4B9F        /* Purple - Features, cards, botones principal */
--primary-dark: #4a3d7a        /* Darker purple - hover, gradients */

/* Colores Secundarios */
--secondary-color: #E74C3C     /* Red - Header, footer, navbar */
--secondary-dark: #C0392B      /* Darker red - hover effects */

/* Colores de Acento */
--accent-color: #FFD700        /* Yellow - Buttons, highlights, CTAs */
--orange-primary: #F39C12      /* Orange - Button hovers */
--orange-secondary: #E67E22    /* Darker orange */

/* Neutrales */
--bg-white: #FFFFFF
--bg-light: #F8F9FA
--bg-medium: #E8EAED
--text-dark: #333333
--text-medium: #666666
--text-light: #999999
--border-color: #E0E0E0
```

#### Tipografía
```css
--font-heading: 'Poppins'       /* Títulos */
--font-body: 'Inter'            /* Texto cuerpo */
--font-weight-light: 300
--font-weight-regular: 400
--font-weight-medium: 500
--font-weight-bold: 700
--font-weight-extra-bold: 800
```

#### Espaciado
```css
--spacing-xs: 0.25rem
--spacing-sm: 0.5rem
--spacing-md: 1rem
--spacing-lg: 1.5rem
--spacing-xl: 2rem
--spacing-2xl: 3rem
```

#### Sombras
```css
--shadow-sm: 0 1px 3px rgba(0, 0, 0, 0.1)
--shadow-md: 0 4px 12px rgba(0, 0, 0, 0.15)
--shadow-lg: 0 20px 60px rgba(0, 0, 0, 0.3)
```

### Archivos CSS Modularizados

| Archivo | Contenido |
|---------|----------|
| `variables.css` | Definiciones CSS, colores, tipografía, espaciado |
| `layout.css` | Header, navbar, footer, main-content, responsive |
| `home.css` | Hero, features, stats, testimonials, pricing, FAQ, CTA |
| `login.css` | Formularios de autenticación, tabs, inputs |
| `modals.css` | OrderDetailModal, TermsAndConditionsModal, PaymentForm |

### Cómo Cambiar Colores Globalmente

Para cambiar la paleta de colores en **toda la app**:

1. Abre `frontend/src/styles/variables.css`
2. Edita los valores en el bloque `:root`:
   ```css
   :root {
     --primary-color: #5B4B9F;     /* Cambiar aquí */
     --secondary-color: #E74C3C;   /* Y aquí */
     --accent-color: #FFD700;      /* Y aquí */
   }
   ```
3. Guarda el archivo
4. **Todos los componentes se actualiza automáticamente**

### Patrón de Importación CSS

Cada componente Vue sigue este patrón:

```vue
<script setup>
// Script logic
</script>

<template>
  <!-- HTML -->
</template>

<style>
@import '../styles/componentName.css';
</style>
```

### Componentes Actualizados al Sistema Global

✅ **Layout:**
- App.vue (header, footer, navbar)

✅ **Páginas:**
- Home.vue (hero, features, pricing)
- Login.vue (auth forms)

✅ **Componentes:**
- OrderDetailModal.vue
- TermsAndConditionsModal.vue
- PaymentForm.vue
- FormField.vue
- Button.vue
- MessengerCard.vue
- MessengerProfile.vue
- MessagePreviewModal.vue
- CallRecordingPlayer.vue
- GoogleSignInButton.vue
- History.vue (earnings table)
- Earnings.vue (withdraw card, stats)

### Responsive Design

Todos los archivos CSS incluyen media queries para:
- **Mobile:** `max-width: 480px`
- **Tablet:** `max-width: 768px`
- **Desktop:** `min-width: 1024px`

---

## 📞 Soporte y Contacto

- **Email:** soporte@badnews.com
- **Issues:** GitHub Issues
- **Documentación:** Ver archivos en `/docs`

---

## 📄 Archivos Importantes

### Documentación Consolidada
- **MASTER.md** (este archivo) - Guía completa
- **database/COMPLETE_DATABASE.sql** - SQL consolidado

### Archivos Desactivados (Reemplazados por MASTER.md)
- README.md (principal)
- FRONTEND_INTEGRATION_STATUS.md
- DATABASE_ARCHITECTURE.md
- GOOGLE_OAUTH_SETUP.md
- Y otros...

---

---

## 🚀 DEPLOYMENT - VERCEL (FRONTEND)

### Estado Actual
- ✅ Frontend deployado en Vercel
- 🌐 URL: https://frontend-zindhers-projects.vercel.app
- 🔄 Auto-deploy desde cambios locales

### Configuración de Vercel
```bash
# Login inicial
npx vercel login

# Deploy a producción
npx vercel --prod

# Ver logs
npx vercel logs
```

### Parámetros Vercel
- **Build Command:** `npm run build`
- **Dev Command:** `npm run dev`
- **Output Directory:** `dist`
- **Framework:** Vite (auto-detectado)

### Próximas pasos
- Conectar dominio personalizado (opcional: gritalo.vercel.app o gritalo.com)
- Configurar variables de entorno para API backend
- Enable CI/CD desde GitHub

---

**Nota:** Este archivo consolidado (`MASTER.md`) reemplaza todos los README/MD existentes. Para información específica, consulta las secciones arriba.

**Última actualización:** 22 de Enero de 2026  
**Versión:** 3.0 - CONSOLIDADO + VERCEL DEPLOYMENT

