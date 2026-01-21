# Deployment Guide

## Production Deployment

This guide covers deploying BadNews to a production environment using Azure services.

## Pre-Deployment Checklist

- [ ] All tests passing (frontend, backend, mobile)
- [ ] Code reviewed and approved
- [ ] Environment variables configured
- [ ] Database migrations reviewed
- [ ] SSL/TLS certificates obtained
- [ ] Secrets securely stored in Azure Key Vault
- [ ] Backup strategy in place
- [ ] Monitoring and alerting configured
- [ ] Load testing completed
- [ ] Security audit completed

## Architecture Overview

```
                    ┌─────────────────┐
                    │  CloudFlare DNS │
                    └────────┬────────┘
                             │
                    ┌────────▼────────┐
                    │  CloudFlare CDN │
                    │   (Caching)     │
                    └────────┬────────┘
                             │
         ┌───────────────────┼───────────────────┐
         │                   │                   │
    ┌────▼────┐        ┌─────▼──────┐      ┌────▼────┐
    │  Frontend     │        │Backend Service│      │ API   │
    │(Static Site) │        │(App Service)   │      │Gateway│
    └─────────┘        └──────────────┘      └───────┘
                             │
         ┌───────────────────┼────────────────┐
         │                   │                │
    ┌────▼───┐          ┌────▼────┐    ┌────▼─────┐
    │SQL DB  │          │ Redis   │    │  Storage │
    │(Primary)          │(Cache)  │    │(Blobs)   │
    └─────────┘         └─────────┘    └──────────┘
```

## Backend Deployment (Azure App Service)

### Prerequisites
- Azure subscription
- Azure CLI installed
- Git configured
- .NET 6+ SDK

### Step 1: Create Azure Resources

```powershell
# Set variables
$resourceGroup = "BadNews-RG"
$appServicePlan = "BadNews-Plan"
$appService = "badnews-api"
$location = "eastus"
$sqlServer = "badnews-sql-server"
$sqlDatabase = "BadNews"
$adminUser = "AdminUser"

# Create resource group
az group create --name $resourceGroup --location $location

# Create App Service Plan
az appservice plan create `
  --name $appServicePlan `
  --resource-group $resourceGroup `
  --location $location `
  --sku S1 `
  --is-linux

# Create App Service
az webapp create `
  --resource-group $resourceGroup `
  --plan $appServicePlan `
  --name $appService `
  --runtime "DOTNET|6.0"

# Create SQL Server
az sql server create `
  --resource-group $resourceGroup `
  --name $sqlServer `
  --location $location `
  --admin-user $adminUser `
  --admin-password $([System.Web.Security.Membership]::GeneratePassword(16, 3))

# Create SQL Database
az sql db create `
  --resource-group $resourceGroup `
  --server $sqlServer `
  --name $sqlDatabase `
  --service-objective S1
```

### Step 2: Configure App Service

```powershell
# Set connection string
az webapp config connection-string set `
  --resource-group $resourceGroup `
  --name $appService `
  --settings DefaultConnection="Server=tcp:$sqlServer.database.windows.net,1433;Initial Catalog=$sqlDatabase;..." `
  --connection-string-type SQLServer

# Set app settings
az webapp config appsettings set `
  --resource-group $resourceGroup `
  --name $appService `
  --settings `
    ASPNETCORE_ENVIRONMENT=Production `
    ALLOWED_HOSTS="*.badnews.com"
```

### Step 3: Deploy Backend

```powershell
# Navigate to backend directory
cd backend

# Build release
dotnet build -c Release

# Deploy to Azure
az webapp deployment source config-zip `
  --resource-group $resourceGroup `
  --name $appService `
  --src "bin/Release/net6.0/publish.zip"
```

### Step 4: Run Database Migrations

```powershell
# Connect to Azure SQL
sqlcmd -S "$sqlServer.database.windows.net" -U $adminUser -P $password -d $sqlDatabase

# Run migrations via Entity Framework
dotnet ef database update --configuration Release
```

## Frontend Deployment (Azure Static Web Apps)

### Prerequisites
- Frontend built with Vite
- GitHub repository configured

### Step 1: Build Frontend

```bash
cd frontend
npm install
npm run build
```

Output will be in `dist/` folder.

### Step 2: Create Static Web App

```powershell
$staticAppName = "badnews-web"

az staticwebapp create `
  --name $staticAppName `
  --resource-group $resourceGroup `
  --source https://github.com/yourusername/badnews `
  --branch main `
  --location $location `
  --app-location "frontend" `
  --output-location "dist" `
  --app-build-command "npm run build"
```

### Step 3: Configure Static App

```powershell
# Enable routing for SPA
az staticwebapp config set `
  --name $staticAppName `
  --resource-group $resourceGroup `
  --file "./staticwebapp.config.json"
```

Create `staticwebapp.config.json`:
```json
{
  "navigationFallback": {
    "rewrite": "/index.html",
    "exclude": ["/api/*", "*.{css,gif,ico,jpg,js,png,svg,webp}"]
  },
  "globalHeaders": {
    "content-security-policy": "default-src 'self'; script-src 'self' 'unsafe-inline' https://cdn.jsdelivr.net; style-src 'self' 'unsafe-inline'; img-src 'self' data: https:;"
  },
  "routes": [
    {
      "route": "/api/*",
      "allowedRoles": ["authenticated"]
    }
  ]
}
```

## Mobile Deployment (Flutter)

### Android Release

```bash
cd mobile

# Build release APK
flutter build apk --release

# Output: build/app/outputs/flutter-apk/app-release.apk
```

**Upload to Google Play:**
1. Create Google Play Developer account
2. Create app in Google Play Console
3. Upload signed APK
4. Fill in store listing
5. Submit for review

### iOS Release

```bash
# Build release IPA
flutter build ipa --release

# Output: build/ios/ipa/
```

**Upload to App Store:**
1. Create Apple Developer account
2. Create app in App Store Connect
3. Upload IPA via Transporter
4. Fill in app information
5. Submit for review

## Docker Deployment

### Build Docker Image

```dockerfile
# Backend
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["backend/BadNews.csproj", "backend/"]
RUN dotnet restore "backend/BadNews.csproj"
COPY . .
WORKDIR "/src/backend"
RUN dotnet build "BadNews.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BadNews.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS=http://+:80
ENTRYPOINT ["dotnet", "BadNews.dll"]
```

### Docker Compose

```yaml
version: '3.8'

services:
  api:
    build:
      context: .
      dockerfile: Dockerfile
    ports:
      - "5000:80"
    environment:
      ASPNETCORE_ENVIRONMENT: Production
      ConnectionStrings__DefaultConnection: "Server=sqlserver;Database=BadNews;User Id=sa;Password=${SA_PASSWORD};"
    depends_on:
      - sqlserver
    networks:
      - badnews-network

  sqlserver:
    image: mcr.microsoft.com/mssql/server:latest
    environment:
      ACCEPT_EULA: "Y"
      SA_PASSWORD: ${SA_PASSWORD}
    ports:
      - "1433:1433"
    volumes:
      - sqldata:/var/opt/mssql
    networks:
      - badnews-network

  redis:
    image: redis:latest
    ports:
      - "6379:6379"
    networks:
      - badnews-network

  frontend:
    build:
      context: ./frontend
      dockerfile: Dockerfile.prod
    ports:
      - "80:80"
    depends_on:
      - api
    networks:
      - badnews-network

volumes:
  sqldata:

networks:
  badnews-network:
    driver: bridge
```

## Environment Configuration

### Production Environment Variables

```env
# Backend
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=https://+:443;http://+:80
ConnectionStrings__DefaultConnection=Server=badnews-sql-server.database.windows.net;Database=BadNews;...

# Security
Jwt__Secret=<long-random-secret-from-keyvault>
Jwt__ExpirationMinutes=1440
ALLOWED_HOSTS=api.badnews.com,api-backup.badnews.com

# Third-party Services
Twilio__AccountSid=<from-keyvault>
Twilio__AuthToken=<from-keyvault>
Twilio__PhoneNumber=+1234567890
SendGrid__ApiKey=<from-keyvault>
SendGrid__FromEmail=noreply@badnews.com
MercadoPago__AccessToken=<from-keyvault>
MercadoPago__PublicKey=<from-keyvault>

# Monitoring
ApplicationInsights__InstrumentationKey=<from-keyvault>
Serilog__MinimumLevel=Information

# Frontend
VITE_API_URL=https://api.badnews.com
VITE_ENVIRONMENT=production
```

## Monitoring & Logging

### Application Insights

```csharp
// In Startup.cs
services.AddApplicationInsightsTelemetry();

var logger = services.AddLogging(logging =>
{
    logging.AddApplicationInsights();
});
```

### Alerts

Configure alerts in Azure Monitor:
- Error rate > 5%
- Response time > 2s
- Database CPU > 80%
- Disk space < 10%

## Scaling

### Horizontal Scaling
```powershell
# Scale out (more instances)
az appservice plan update `
  --name $appServicePlan `
  --resource-group $resourceGroup `
  --sku P1V2 `
  --number-of-workers 3
```

### Database Scaling
```powershell
# Scale up database
az sql db update `
  --resource-group $resourceGroup `
  --server $sqlServer `
  --name $sqlDatabase `
  --service-objective P2
```

## Backup & Recovery

### Database Backup
```powershell
# Configure automatic backups (built-in for Azure SQL)
az sql db update `
  --resource-group $resourceGroup `
  --server $sqlServer `
  --name $sqlDatabase `
  --backup-storage-redundancy Geo
```

### Restore from Backup
```powershell
az sql db restore `
  --resource-group $resourceGroup `
  --server $sqlServer `
  --name $sqlDatabase `
  --target-server-name $sqlServer `
  --target-database-name BadNews-Restored `
  --time "2026-01-21T12:00:00Z"
```

## SSL/TLS Certificate

### Add Custom Domain
```powershell
az appservice web config hostname add `
  --webapp-name $appService `
  --resource-group $resourceGroup `
  --hostname api.badnews.com
```

### Configure SSL
```powershell
# Upload certificate
az appservice web config ssl upload `
  --certificate-file certificate.pfx `
  --certificate-password $password `
  --name $appService `
  --resource-group $resourceGroup

# Bind to hostname
az appservice web config ssl bind `
  --certificate-thumbprint $thumbprint `
  --ssl-type SNI `
  --name $appService `
  --resource-group $resourceGroup
```

## CI/CD Pipeline

Create `.github/workflows/deploy.yml`:

```yaml
name: Deploy to Production

on:
  push:
    branches: [main]

jobs:
  build-and-deploy:
    runs-on: ubuntu-latest
    
    steps:
    - uses: actions/checkout@v2
    
    - name: Setup .NET
      uses: actions/setup-dotnet@v1
      with:
        dotnet-version: '6.0.x'
    
    - name: Build Backend
      run: dotnet build -c Release
    
    - name: Test Backend
      run: dotnet test -c Release
    
    - name: Publish Backend
      run: dotnet publish backend -c Release -o publish
    
    - name: Deploy to Azure
      uses: azure/appservice-deploy@v2
      with:
        app-name: badnews-api
        publish-profile: ${{ secrets.AZURE_PUBLISH_PROFILE }}
        package: publish
```

## Rollback Strategy

### Deployment Slots

```powershell
# Create staging slot
az webapp deployment slot create `
  --resource-group $resourceGroup `
  --name $appService `
  --slot staging

# Deploy to staging
# (run deployment script targeting staging)

# Swap to production
az webapp deployment slot swap `
  --resource-group $resourceGroup `
  --name $appService `
  --slot staging

# Swap back if needed
az webapp deployment slot swap `
  --resource-group $resourceGroup `
  --name $appService `
  --slot staging `
  --action swap
```

## Health Checks

### Backend Health Endpoint

```csharp
app.MapGet("/health", async (IServiceProvider services) =>
{
    var db = services.GetRequiredService<AppDbContext>();
    
    try
    {
        await db.Database.ExecuteSqlAsync($"SELECT 1");
        return Results.Ok(new { status = "Healthy", timestamp = DateTime.UtcNow });
    }
    catch
    {
        return Results.StatusCode(503);
    }
});
```

### Configure Health Check Probe

```powershell
az appservice web config set `
  --name $appService `
  --resource-group $resourceGroup `
  --generic-configurations @{health_check_path = "/health"}
```

## Security Hardening

### Enable HTTPS Only
```powershell
az webapp update `
  --resource-group $resourceGroup `
  --name $appService `
  --https-only true
```

### Configure CORS
```powershell
az webapp cors add `
  --resource-group $resourceGroup `
  --name $appService `
  --allowed-origins https://badnews.com https://www.badnews.com
```

### IP Restrictions
```powershell
# Restrict to CloudFlare IPs
az webapp config access-restriction add `
  --resource-group $resourceGroup `
  --name $appService `
  --rule-name CloudFlareOnly `
  --priority 100 `
  --ip-address 103.21.244.0/22
```

## Troubleshooting

### Check Logs
```powershell
# Stream logs
az webapp log tail --resource-group $resourceGroup --name $appService

# Download logs
az webapp log download --resource-group $resourceGroup --name $appService
```

### Common Issues

**Issue: Connection String Invalid**
- Verify firewall rules allow connection
- Check credentials in Key Vault
- Ensure database is accessible from App Service

**Issue: High Memory Usage**
- Check for memory leaks in application code
- Review Hangfire job queue
- Increase App Service plan size

**Issue: Slow Database Queries**
- Check query plans in SQL Server Management Studio
- Add missing indexes
- Enable query result caching with Redis

---

**Last Updated:** January 21, 2026
