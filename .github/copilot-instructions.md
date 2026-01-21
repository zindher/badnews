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

## Notas Importantes
- La grabación de llamadas es requisito crítico
- Sistema de reintentos: 3 llamadas/día por 3 días
- Soporte para anonimato de compradores
- Integración con SMS/Email como fallback
