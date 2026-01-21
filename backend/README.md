# BadNews Backend

API REST con .NET Core Web API

## Setup

```bash
cd backend
dotnet restore
dotnet build
dotnet run
```

## Configuración

Editar `appsettings.json` con:
- Conexión SQL Server
- Credenciales Twilio
- Credenciales Mercado Pago
- API Key SendGrid
- Secreto JWT

## Base de Datos

```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

## Estructura

- `Models/` - Entidades de base de datos
- `Controllers/` - Endpoints API
- `Services/` - Lógica de negocio
- `Data/` - DbContext y configuración
- `Configurations/` - Configuraciones globales
