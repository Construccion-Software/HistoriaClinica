# Etapa 1: Build (compilar la aplicación)
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

# Copiar archivos del proyecto
COPY ["HistoriasClinicas.Api.csproj", "."]

# Restaurar dependencias
RUN dotnet restore "HistoriasClinicas.Api.csproj"

# Copiar código fuente
COPY . .

# Compilar la aplicación
RUN dotnet build "HistoriasClinicas.Api.csproj" -c Release -o /app/build

# Publicar
RUN dotnet publish "HistoriasClinicas.Api.csproj" -c Release -o /app/publish

# Etapa 2: Runtime (ejecutar la aplicación)
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS runtime
WORKDIR /app

# Copiar la aplicación compilada desde la etapa build
COPY --from=build /app/publish .

# Copiar archivos de configuración
COPY appsettings.json .
COPY appsettings.Development.json .
COPY appsettings.Production.json .

# Exponer el puerto 5000 (HTTP)
EXPOSE 5000

# Variables de entorno por defecto
ENV ASPNETCORE_URLS=http://+:5000
ENV ASPNETCORE_ENVIRONMENT=Production

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD dotnet /app/HistoriasClinicas.Api.dll || exit 1

# Comando de inicio
ENTRYPOINT ["dotnet", "HistoriasClinicas.Api.dll"]
