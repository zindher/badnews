# 📚 DOCUMENTACIÓN - GUÍA DE LECTURA

**Última actualización:** 21 de Enero de 2026

---

## 🎯 LEER PRIMERO

### Para Entender Todo el Proyecto
→ **[MASTER.md](./MASTER.md)** (Documento maestro consolidado)
- Stack tecnológico
- Estructura completa
- Setup rápido
- APIs
- Google OAuth
- Autenticación
- Troubleshooting

---

## 💾 BASE DE DATOS

### Setup de Base de Datos
→ **[database/COMPLETE_DATABASE.sql](./database/COMPLETE_DATABASE.sql)**
- Todas las tablas (10)
- Índices y constraints
- Google OAuth fields
- Procedimientos almacenados

**Cómo usar:**
```sql
-- Abre SQL Server Management Studio
-- Conecta a tu servidor SQL Server
-- Copia y ejecuta el contenido de COMPLETE_DATABASE.sql
-- La BD se creará automáticamente
```

---

## 🔥 ARCHIVOS DESACTIVADOS

Los siguientes archivos están reemplazados por **MASTER.md**:
- ❌ README.md
- ❌ README_IMPLEMENTATION.md
- ❌ FRONTEND_INTEGRATION_STATUS.md
- ❌ FRONTEND_FINAL_REPORT.md
- ❌ FRONTEND_FILE_STRUCTURE.md
- ❌ FRONTEND_STATUS_REPORT.md
- ❌ DATABASE_ARCHITECTURE.md
- ❌ DATABASE_COMPONENTS_SUMMARY.md
- ❌ DATABASE_CREATION_SUMMARY.md
- ❌ DATABASE_IMPLEMENTATION_COMPLETE.md
- ❌ DATABASE_READY_SUMMARY.md
- ❌ DATABASE_REVIEW_COMPLETE.md
- ❌ GOOGLE_OAUTH_SETUP.md
- ❌ SESSION_SUMMARY.md
- ❌ WORK_SUMMARY_DATABASE.md
- ❌ COMPLETION_SUMMARY.md
- ❌ COMPLETION_SUMMARY_FRONTEND.md
- ❌ USER_ARCHITECTURE.md
- ❌ CHANGES_v2.1.md
- ❌ IMPLEMENTATION_CHECKLIST.md
- ❌ NEXT_STEPS.md
- ❌ NEXT_STEPS_API_DEVELOPMENT.md
- ❌ PENDING_TASKS.md
- ❌ QUICKSTART.md
- ❌ MOBILE_IMPROVEMENTS.md

**¿Por qué?** Todos estos temas están consolidados en MASTER.md con una mejor organización.

---

## 📖 DOCUMENTACIÓN EN /docs/ (Heredada)

Los archivos en `/docs/` son de referencia y pueden estar desactualizados:
- docs/SETUP.md
- docs/API.md
- docs/ARCHITECTURE.md
- docs/DATABASE_SETUP.md
- docs/DATABASE_ARCHITECTURE.md
- docs/COMPLETE_DATABASE_REVIEW.md
- docs/DEPLOYMENT.md
- docs/MIGRATIONS_GUIDE.md
- docs/USER_FLOW_DIAGRAMS.md
- docs/USER_TYPES.md
- docs/CONTRIBUTING.md

**→ Usar MASTER.md en su lugar**

---

## 🚀 EMPEZAR RÁPIDO

### 1️⃣ Lee MASTER.md (5 minutos)
- Entenderás la arquitectura completa
- Sabrás qué va en cada carpeta
- Conocerás todos los endpoints

### 2️⃣ Ejecuta COMPLETE_DATABASE.sql
```bash
# SQL Server Management Studio
# New Query → Paste contenido → Execute
# Base de datos se crea automáticamente
```

### 3️⃣ Ejecuta Backend
```bash
cd backend
dotnet restore
dotnet ef database update  # Si usas EF Migrations
dotnet run
# http://localhost:5000
```

### 4️⃣ Ejecuta Frontend
```bash
cd frontend
npm install
npm run dev
# http://localhost:5173
```

---

## 📋 ESTRUCTURA DE CARPETAS

```
BadNews/
├── MASTER.md                         ← 📌 LEE PRIMERO
├── DOCUMENTATION_GUIDE.md            ← Tú estás aquí
├── VERCEL_QUICKSTART.md              ← 🚀 Deploy rápido a Vercel
├── VERCEL_DEPLOYMENT.md              ← 📦 Documentación completa de Vercel
├── database/
│   └── COMPLETE_DATABASE.sql         ← 💾 TODO el SQL
├── backend/
│   ├── Models/
│   ├── Controllers/
│   ├── Services/
│   ├── Program.cs
│   └── appsettings.json
├── frontend/
│   ├── src/
│   │   ├── components/
│   │   ├── pages/
│   │   ├── services/
│   │   └── stores/
│   ├── .env.local
│   └── package.json
├── mobile/
│   └── lib/
└── docs/
    └── [DEPRECATED - ver MASTER.md]
```

---

## 🔍 BUSCAR POR TEMA

### Necesito...

**Configurar Google OAuth**
→ MASTER.md → Sección "Google OAuth"

**Entender la base de datos**
→ MASTER.md → Sección "Base de Datos"

**Saber qué endpoints tengo**
→ MASTER.md → Sección "API Endpoints"

**Configurar autenticación**
→ MASTER.md → Sección "Autenticación"

**Resolver un error**
→ MASTER.md → Sección "Troubleshooting"

**Configurar Despliegue en Vercel**
→ VERCEL_QUICKSTART.md (guía rápida)
→ VERCEL_DEPLOYMENT.md (documentación completa)

**Saber qué tecnologías se usan**
→ MASTER.md → Sección "Stack Tecnológico"

**Setup rápido**
→ MASTER.md → Sección "Setup Rápido"

**Ver roles de usuario**
→ MASTER.md → Sección "Roles y Permisos"

---

## ✅ Checklist de Setup

- [ ] Leer MASTER.md
- [ ] Ejecutar COMPLETE_DATABASE.sql
- [ ] Configurar appsettings.json (backend)
- [ ] Configurar .env.local (frontend)
- [ ] `cd backend && dotnet restore`
- [ ] `cd backend && dotnet run`
- [ ] `cd frontend && npm install`
- [ ] `cd frontend && npm run dev`
- [ ] Verificar http://localhost:5173
- [ ] Probar login/registro

---

## 📞 Duda Frecuentes

**P: ¿Dónde está el README antiguo?**
A: Consolidado en MASTER.md

**P: ¿Dónde está la documentación de Google OAuth?**
A: MASTER.md → "Google OAuth"

**P: ¿Qué archivos SQL necesito ejecutar?**
A: Solo uno: `database/COMPLETE_DATABASE.sql`

**P: ¿Qué documentación está desactualizada?**
A: Todo lo en `/docs/` y los múltiples archivos MD. Usa MASTER.md

**P: ¿Dónde veo los endpoints API?**
A: MASTER.md → "API Endpoints"

---

## 🎓 Aprendizaje Progresivo

1. **Principiante**: Lee MASTER.md sección "Setup Rápido"
2. **Intermedio**: Lee MASTER.md completo
3. **Avanzado**: Revisa código en `/backend` y `/frontend`
4. **DevOps**: Lee MASTER.md sección "Deployment"

---

## 📌 Versión Actual

**MASTER.md v2.0 - CONSOLIDADO**
- Única fuente de verdad
- Actualizado al 21 de Enero de 2026
- Incluye Google OAuth
- Incluye toda la BD

---

**Recomendación:** Marca esta página como referencia y siempre consulta MASTER.md primero. 🚀

