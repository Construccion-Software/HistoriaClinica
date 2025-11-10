# 🐳 Guía Docker - HistoriasClinicas.Api

## Requisitos Previos

- **Docker Desktop** instalado ([Descargar](https://www.docker.com/products/docker-desktop))
- **Docker Compose** (incluido en Docker Desktop)

---

## 📁 Estructura de Archivos

```
HistoriasClinicas.Api/
├── Dockerfile                 # Imagen Docker multietapa
├── .dockerignore             # Archivos a excluir del contexto
├── docker-compose.yml        # Desarrollo local
├── docker-compose.prod.yml   # Producción con MongoDB Atlas
├── .env.docker               # Variables para desarrollo
├── .env                       # Variables de producción (NO comitear)
├── scripts/
│   └── mongo-init.js         # Script inicialización MongoDB
└── appsettings.*.json        # Configuraciones por ambiente
```

---

## 🚀 **Ejecución LOCAL (Desarrollo)**

### Opción 1: Con Docker Compose (MongoDB local)

```bash
# 1. Navega al directorio del proyecto
cd c:\Construccion\HistoriasClinicas.Api

# 2. Inicia los servicios
docker-compose --env-file .env.docker up -d

# 3. Ver logs
docker-compose logs -f historias-clinicas-api

# 4. Acceder a la API
http://localhost:5000/swagger
```

**Credenciales MongoDB local:**
- Usuario: `admin`
- Contraseña: `mongopassword123`
- Connection String: `mongodb://admin:mongopassword123@localhost:27017/HistoriasClinicasDb`

---

### Opción 2: Con Docker Compose - Construcción desde cero

```bash
# Reconstruir imagen
docker-compose --env-file .env.docker build

# Iniciar servicios
docker-compose --env-file .env.docker up
```

---

## 🌐 **Ejecución PRODUCCIÓN (MongoDB Atlas)**

### Paso 1: Crear archivo `.env.prod`

```bash
# .env.prod
MONGODB_CONNECTION_STRING=mongodb+srv://jandersondev89_db_user:OyALtuNwUobZYvIf@cluster0.0xbaomn.mongodb.net/?appName=Cluster0
MONGODB_DATABASE_NAME=HistoriasClinicasDb
MONGODB_COLLECTION_NAME=HistoriasClinicas
ASPNETCORE_ENVIRONMENT=Production
```

### Paso 2: Ejecutar en producción

```bash
# Construir imagen
docker build -t historias-clinicas-api:prod .

# Ejecutar con docker-compose.prod.yml
docker-compose -f docker-compose.prod.yml --env-file .env.prod up -d
```

O manualmente:

```bash
docker run -d \
  --name historias-clinicas-api-prod \
  -p 5000:5000 \
  -e ASPNETCORE_ENVIRONMENT=Production \
  -e MONGODB_CONNECTION_STRING="mongodb+srv://..." \
  -e MONGODB_DATABASE_NAME=HistoriasClinicasDb \
  -e MONGODB_COLLECTION_NAME=HistoriasClinicas \
  --restart always \
  historias-clinicas-api:prod
```

---

## 🛠️ Comandos Útiles

### Ver estado de contenedores

```bash
docker-compose ps
```

### Ver logs en tiempo real

```bash
docker-compose logs -f
```

### Detener servicios

```bash
docker-compose down
```

### Detener y eliminar volúmenes (⚠️ elimina datos)

```bash
docker-compose down -v
```

### Acceder a la shell de MongoDB

```bash
docker exec -it historias-clinicas-mongodb mongosh -u admin -p mongopassword123
```

### Acceder a la shell del contenedor API

```bash
docker exec -it historias-clinicas-api bash
```

### Ver imagen creada

```bash
docker images | grep historias-clinicas
```

### Eliminar imagen

```bash
docker rmi historias-clinicas-api:prod
```

---

## 📊 Volúmenes y Persistencia

### Volumen de MongoDB

```yaml
volumes:
  mongodb_data:
    driver: local
```

Los datos se persisten en `./data/mongodb` (o en Docker Desktop: `~/.docker/volumes/`)

### Ver volúmenes

```bash
docker volume ls
```

### Inspeccionar volumen

```bash
docker volume inspect historias-clinicas-mongodb-data
```

---

## 🌐 Redes Docker

Todos los servicios están en la red `historias-clinicas-network`:

```yaml
networks:
  historias-clinicas-network:
    driver: bridge
```

**Comunicación entre servicios:**
- Dentro del Docker: `http://mongodb:27017`
- Desde el host: `http://localhost:27017`

---

## 📝 Health Checks

La API tiene un health check configurado:

```bash
curl http://localhost:5000/health
```

**Nota:** Asegúrate que tu `HistoriasClinicasController` tenga un endpoint `/health`:

```csharp
[HttpGet("health")]
public IActionResult Health()
{
    return Ok(new { status = "healthy", timestamp = DateTime.UtcNow });
}
```

---

## 🔒 Seguridad en Producción

### Buenas prácticas:

1. **No comitees `.env.prod`** - Usa secrets management (GitHub Secrets, Azure Key Vault, etc.)
2. **Usa variables de entorno** - Desde el host o CI/CD
3. **Cambia credenciales por defecto** - MongoDB, API keys, etc.
4. **Usa redes privadas** - En Kubernetes o Docker Swarm
5. **Implementa HTTPS** - Con certificados SSL/TLS
6. **Limita recursos** - CPU, memoria, etc.

---

## 🐛 Troubleshooting

### Error: "mongodb: name or service not known"

**Solución:** Asegúrate que MongoDB está iniciado:
```bash
docker-compose up -d mongodb
docker-compose logs mongodb
```

### Error: "Connection refused to MongoDB"

**Solución:** Espera a que MongoDB esté listo:
```bash
docker-compose up --wait
```

### Error: "EADDRINUSE: address already in use :::5000"

**Solución:** El puerto 5000 ya está en uso. Cambia en `docker-compose.yml`:
```yaml
ports:
  - "5001:5000"  # Usa 5001 en lugar de 5000
```

### Error: "DotNetEnv not found"

**Solución:** Ejecuta `dotnet restore` antes de construir:
```bash
dotnet restore
docker build .
```

---

## 📚 Referencias

- [Docker Documentation](https://docs.docker.com/)
- [Docker Compose Reference](https://docs.docker.com/compose/compose-file/)
- [.NET on Docker](https://docs.microsoft.com/en-us/dotnet/architecture/microservices/container-docker-introduction/)
- [MongoDB Docker Hub](https://hub.docker.com/_/mongo)

---

## 🎯 Próximos Pasos

1. **Añadir más microservicios** en otros lenguajes
2. **Implementar Kubernetes** para orquestación
3. **Configurar CI/CD** (GitHub Actions, Azure Pipelines)
4. **Implementar API Gateway** (Kong, Nginx)
5. **Agregar logging centralizado** (ELK Stack)

---

**¿Preguntas?** Consulta la documentación oficial de Docker.
