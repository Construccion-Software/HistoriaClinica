# 🐳 Publicar Imagen Docker en Docker Hub con GitHub Actions

## Prerequisitos

1. **Cuenta en Docker Hub** - Regístrate en https://hub.docker.com
2. **Acceso al repositorio GitHub** - Tienes permisos de admin
3. **Token de Docker Hub** - Para autenticación

---

## 📝 Paso 1: Crear Token en Docker Hub

1. Inicia sesión en https://hub.docker.com
2. Ve a **Account Settings** → **Security**
3. Haz clic en **New Access Token**
4. Nombre: `github-actions`
5. Permisos: `Read & Write`
6. Copia el token (⚠️ no lo compartas)

---

## 🔐 Paso 2: Agregar Secretos en GitHub

1. Ve a tu repositorio en GitHub
2. Settings → **Secrets and variables** → **Actions**
3. Haz clic en **New repository secret**
4. Agrega estos 2 secretos:

### Secret 1: `DOCKER_USERNAME`
- **Value:** `anderson12099` (tu usuario de Docker Hub)

### Secret 2: `DOCKER_PASSWORD`
- **Value:** El token que copiaste en Paso 1

---

## 🚀 Paso 3: Trigger del Build

El workflow se ejecutará automáticamente cuando:

1. **Hagas push a `MergeCalidad`** (rama actual)
2. **Hagas push a `main` o `master`**
3. **Crees un tag** (ej: `v1.0.0`)
4. **Ejecutes manualmente** desde Actions

---

## 📊 Monitoreo

### Ver el build en tiempo real:
1. Ve a tu repositorio GitHub
2. Haz clic en **Actions**
3. Verás el workflow `Build and Push Docker Image` ejecutándose

### Después de completar:
- La imagen estará en `docker.io/anderson12099/historias-clinicas-api`
- Tags automáticos:
  - `MergeCalidad` (rama)
  - `latest` (si es default branch)
  - `vX.X.X` (si creas tags)

---

## 🎯 Tags Generados Automáticamente

| Evento | Tags Generados |
|--------|----------------|
| Push a MergeCalidad | `MergeCalidad`, `latest` |
| Push a main | `main` |
| Tag v1.2.3 | `1.2.3`, `1.2`, `1` |
| Commit SHA | `MergeCalidad-abc123def` |

---

## ✅ Verificar Imagen en Docker Hub

Después que termine:
1. Ve a https://hub.docker.com/r/anderson12099/historias-clinicas-api
2. Verás la imagen con los tags

### Usar la imagen:
```bash
docker pull anderson12099/historias-clinicas-api:MergeCalidad
docker run -p 5000:5000 \
  -e MONGODB_CONNECTION_STRING="mongodb+srv://..." \
  anderson12099/historias-clinicas-api:MergeCalidad
```

---

## 🐛 Troubleshooting

### Error: "Authentication failed"
- Verifica que `DOCKER_USERNAME` y `DOCKER_PASSWORD` sean correctos
- El token debe estar vigente

### Error: "Build context not found"
- Asegúrate que el `Dockerfile` está en la raíz del repo
- El archivo `.dockerignore` está presente

### La imagen no aparece en Docker Hub
- Ve a Actions y verifica que el workflow completó ✅
- Recarga la página de Docker Hub

---

## 📚 Recursos

- [GitHub Actions Documentation](https://docs.github.com/en/actions)
- [Docker Build Action](https://github.com/docker/build-push-action)
- [Docker Hub API](https://docs.docker.com/docker-hub/api/)

---

**¡Tu imagen se publicará automáticamente en cada push!** 🚀
