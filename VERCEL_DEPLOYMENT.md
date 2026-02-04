# Configuración de Despliegue en Vercel

Este documento explica cómo configurar el despliegue automático del frontend de BadNews en Vercel.

## 🚀 Configuración Inicial

### 1. Conectar el Repositorio con Vercel

1. Ve a [Vercel Dashboard](https://vercel.com/dashboard)
2. Click en "Add New Project"
3. Importa el repositorio `zindher/badnews`
4. Configura el proyecto con los siguientes parámetros:

   - **Framework Preset:** Vite
   - **Root Directory:** `frontend`
   - **Build Command:** `npm run build`
   - **Output Directory:** `dist`
   - **Install Command:** `npm ci`

### 2. Configurar Variables de Entorno en Vercel

Agrega las siguientes variables de entorno en tu proyecto de Vercel:

#### Production Environment:
```
VITE_API_URL=https://api.badnews.com
VITE_APP_NAME=BadNews
VITE_ENABLE_ANALYTICS=true
```

#### Preview Environment (opcional):
```
VITE_API_URL=https://api-staging.badnews.com
VITE_APP_NAME=BadNews (Preview)
VITE_ENABLE_ANALYTICS=false
```

### 3. Configurar Secretos en GitHub

Para habilitar el despliegue automático mediante GitHub Actions, necesitas agregar el token de Vercel:

1. Ve a [Vercel Account Settings > Tokens](https://vercel.com/account/tokens)
2. Crea un nuevo token
3. Ve a los Settings de tu repositorio en GitHub
4. Navega a "Secrets and variables" > "Actions"
5. Agrega el siguiente secreto:
   - **Name:** `VERCEL_TOKEN`
   - **Value:** (el token que generaste en Vercel)

### 4. Obtener el ID del Proyecto (opcional para CLI)

Si necesitas el ID del proyecto de Vercel:

```bash
cd frontend
vercel link
```

Esto creará un archivo `.vercel/project.json` con los IDs necesarios.

## 🔄 Despliegue Automático

### Mediante Git Push

Una vez configurado, el despliegue es automático:

- **Push a `main`:** Despliega a producción en Vercel
- **Push a `develop`:** Despliega un preview en Vercel
- **Pull Request:** Crea un preview deployment único

### Mediante Vercel CLI (Manual)

Para despliegue manual local:

```bash
# Instalar Vercel CLI
npm i -g vercel

# Desplegar a preview
cd frontend
vercel

# Desplegar a producción
cd frontend
vercel --prod
```

## 📋 Workflows Configurados

### `.github/workflows/vercel-deploy.yml`

Este workflow:
- Se activa en push a `main` o `develop`
- Se activa en pull requests
- Solo se ejecuta si hay cambios en `frontend/**`
- Despliega automáticamente a Vercel
- Comenta en PRs con la URL del preview

## 🔗 URLs de Despliegue

- **Producción:** https://frontend-zindhers-projects.vercel.app
- **Preview (develop):** Se genera automáticamente
- **PR Previews:** Se genera un URL único para cada PR

## 🛠️ Configuración de Dominio Personalizado

Para agregar un dominio personalizado:

1. Ve a tu proyecto en Vercel Dashboard
2. Navega a "Settings" > "Domains"
3. Agrega tu dominio (ej: `badnews.mx`, `www.badnews.mx`)
4. Configura los DNS según las instrucciones de Vercel

### DNS Records Recomendados:
```
Type    Name    Value
A       @       76.76.21.21
CNAME   www     cname.vercel-dns.com
```

## 🔍 Monitoreo y Logs

- **Deployment Logs:** Visibles en Vercel Dashboard
- **Runtime Logs:** Disponibles en la pestaña "Functions" de cada deployment
- **Analytics:** Habilitados automáticamente en producción

## ⚠️ Troubleshooting

### Error: "Build Failed"
- Verifica que las variables de entorno estén configuradas
- Revisa los logs del build en Vercel Dashboard
- Asegúrate de que `npm run build` funciona localmente

### Error: "Page Not Found"
- Verifica que `outputDirectory` sea `dist`
- Revisa la configuración de rewrites en `vercel.json`

### Variables de Entorno no Funcionan
- Asegúrate de que las variables empiecen con `VITE_`
- Reinicia el deployment después de agregar nuevas variables
- Las variables solo se aplican en el siguiente deployment

## 📚 Referencias

- [Vercel Documentation](https://vercel.com/docs)
- [Vite Environment Variables](https://vitejs.dev/guide/env-and-mode.html)
- [GitHub Actions for Vercel](https://vercel.com/guides/how-can-i-use-github-actions-with-vercel)

## ✅ Checklist de Configuración

- [ ] Proyecto conectado en Vercel
- [ ] Variables de entorno configuradas en Vercel
- [ ] Token de Vercel agregado a GitHub Secrets
- [ ] Primer despliegue exitoso
- [ ] Dominio personalizado configurado (opcional)
- [ ] Webhooks configurados (opcional)
