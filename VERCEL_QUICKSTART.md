# 🚀 Guía Rápida: Conectar Frontend con Vercel

## Pasos Inmediatos para Conectar

### 1️⃣ Ir a Vercel Dashboard
Visita: https://vercel.com/zindhers-projects/frontend

### 2️⃣ Conectar el Repositorio (Si aún no está conectado)

1. En Vercel Dashboard, click en **"Import Project"**
2. Selecciona el repositorio: `zindher/badnews`
3. Configura:
   ```
   Framework Preset: Vite
   Root Directory: frontend
   Build Command: npm run build
   Output Directory: dist
   Install Command: npm ci
   ```

### 3️⃣ Agregar Variables de Entorno en Vercel

Ve a tu proyecto en Vercel > Settings > Environment Variables

**Variables Requeridas:**
```env
VITE_API_URL=https://api.badnews.com
VITE_APP_NAME=BadNews
VITE_ENABLE_ANALYTICS=true
```

### 4️⃣ Configurar GitHub Actions (Despliegue Automático)

1. Ve a https://vercel.com/account/tokens
2. Crea un nuevo token llamado `badnews-github-actions`
3. Copia el token

En tu repositorio GitHub:
1. Ve a Settings > Secrets and variables > Actions
2. Click en "New repository secret"
3. Agrega:
   - **Name:** `VERCEL_TOKEN`
   - **Secret:** (pega el token de Vercel)

### 5️⃣ ¡Listo! Ahora el Despliegue es Automático

- ✅ Push a `main` → Despliega a producción
- ✅ Push a `develop` → Despliega preview
- ✅ Pull Request → Crea preview único con URL

## 📋 Verificación

### Revisar que esté funcionando:

```bash
# Ver archivos de configuración
ls -la vercel.json
ls -la frontend/vercel.json
ls -la .github/workflows/vercel-deploy.yml

# Ver el último commit
git log -1

# Push para probar (esto disparará el workflow)
git push origin main
```

### Verificar en GitHub Actions:
1. Ve a: https://github.com/zindher/badnews/actions
2. Busca el workflow "Deploy to Vercel"
3. Verifica que se ejecute correctamente

## 🔗 URLs Importantes

- **Proyecto en Vercel:** https://vercel.com/zindhers-projects/frontend
- **GitHub Actions:** https://github.com/zindher/badnews/actions
- **Documentación Completa:** Ver `VERCEL_DEPLOYMENT.md`

## ⚡ Primer Despliegue Manual (Opcional)

Si quieres hacer el primer despliegue manualmente:

```bash
# Instalar Vercel CLI
npm i -g vercel

# Ir al directorio del frontend
cd frontend

# Login a Vercel
vercel login

# Link al proyecto existente
vercel link

# Deploy a producción
vercel --prod
```

## 🎯 Siguiente Paso: ¡Hacer Push!

Con estos archivos ya configurados, simplemente haz:

```bash
git push origin main
```

Y el frontend se desplegará automáticamente a Vercel! 🎉

---

**Nota:** Todos los archivos de configuración ya están creados y listos. Solo necesitas:
1. Agregar el `VERCEL_TOKEN` a GitHub Secrets
2. Configurar las variables de entorno en Vercel
3. ¡Hacer push y ver la magia!
