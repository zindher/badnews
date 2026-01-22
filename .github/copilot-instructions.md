# BadNews Project - Copilot Instructions

## Descripción General
BadNews es una aplicación full-stack que conecta a compradores con mensajeros para realizar llamadas personalizadas. Incluye:
- Backend: .NET Core Web API
- Frontend: Vue 3 + Vite
- Mobile: Flutter
- Base de datos: SQL Server

## Stack Tecnológico Confirmado
- **Backend:** C# Web API con .NET Core
- **Frontend:** Vue 3 + Vite
- **Mobile:** Flutter (Dart)
- **Base de Datos:** SQL Server
- **Servicios:** Twilio, Mercado Pago, SendGrid

## Proyecto Checklist

- [ ] Crear estructura Backend .NET Core
- [ ] Crear estructura Frontend Vue 3 + Vite
- [ ] Crear estructura Mobile Flutter
- [ ] Configurar SQL Server y Entity Framework Core
- [ ] Implementar modelos de datos
- [ ] Crear API endpoints básicos
- [ ] Integrar Twilio
- [ ] Integrar Mercado Pago
- [ ] Integrar SendGrid
- [ ] Sistema de reintentos de llamadas
- [ ] Autenticación y autorización
- [ ] Componentes Vue 3
- [ ] Flutter UI para Mensajeros
- [ ] Tests y validaciones

## Guías de Desarrollo

### Antes de empezar cualquier tarea
1. Lee esta guía completamente
2. Confirma los requisitos específicos con el usuario
3. Utiliza la herramienta manage_todo_list para tracking

### Convenciones de Código
- Backend: Namespaces siguiendo estructura de carpetas
- Frontend: Componentes en PascalCase
- Mobile: Nombres en camelCase

### Estructura de Carpetas Esperada
```
backend/
  ├── Models/
  ├── Controllers/
  ├── Services/
  ├── Data/
  └── Configurations/

frontend/
  ├── src/
  │   ├── components/
  │   ├── pages/
  │   ├── services/
  │   └── App.vue
  └── vite.config.js

mobile/
  ├── lib/
  │   ├── models/
  │   ├── screens/
  │   ├── services/
  │   └── main.dart
  └── pubspec.yaml
```

## ⚠️ POLÍTICA DE DOCUMENTACIÓN (CRÍTICO)

### NO CREAR MÁS ARCHIVOS MD/README
- ❌ **NO crear** archivos README.md adicionales
- ❌ **NO crear** archivos de documentación nuevos (.md)
- ❌ **NO crear** archivos de resumen o status
- ❌ **NO duplicar** información existente

### Archivos MD Permitidos (SOLO estos)
- ✅ **MASTER.md** - Documentación maestra única (única fuente de verdad)
- ✅ **DOCUMENTATION_GUIDE.md** - Índice de lectura y navegación
- ✅ **database/COMPLETE_DATABASE.sql** - SQL consolidado
- ✅ **Archivos en docs/** - Solo mantenimiento heredado

### Si necesitas documentar algo:
1. Actualiza **MASTER.md** (agregar nueva sección)
2. Si es SQL → actualiza **database/COMPLETE_DATABASE.sql**
3. Si es búsqueda → actualiza **DOCUMENTATION_GUIDE.md**
4. **NUNCA** crees un archivo MD nuevo

### Consecuencia
Si se crea un archivo MD no autorizado, debe ser eliminado inmediatamente.

## Notas Importantes
- La grabación de llamadas es requisito crítico
- Sistema de reintentos: 3 llamadas/día por 3 días
- Soporte para anonimato de compradores
- Integración con SMS/Email como fallback
