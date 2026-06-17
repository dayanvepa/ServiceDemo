# ── Etapa 1: Build ──────────────────────────────────────────
# pero mantenemos la imagen base sdk:9.0 normal para asegurar compatibilidad con herramientas de compilación.
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# 1. Copiar archivos de solución y proyectos para aprovechar el caché de capas
COPY ServiceDemo.slnx ./
COPY src/ServiceDemo.Domain/ServiceDemo.Domain.csproj             src/ServiceDemo.Domain/
COPY src/ServiceDemo.Application/ServiceDemo.Application.csproj   src/ServiceDemo.Application/
COPY src/ServiceDemo.Infrastructure/ServiceDemo.Infrastructure.csproj src/ServiceDemo.Infrastructure/
COPY src/ServiceDemo.API/ServiceDemo.API.csproj                   src/ServiceDemo.API/

# 2. Restaurar dependencias
RUN dotnet restore src/ServiceDemo.API/ServiceDemo.API.csproj

# 3. Copiar el resto del código y publicar
COPY src/ src/
RUN dotnet publish src/ServiceDemo.API/ServiceDemo.API.csproj \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false

# ── Etapa 2: Runtime (Seguro y Optimizado) ──────────────────
# Usamos 'aspnet:9.0-alpine' para reducir la superficie de ataque y el tamaño de la imagen
FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine AS runtime
WORKDIR /app

# PATCH: Actualizar librerías del sistema con fixes de seguridad
RUN apk add --no-cache --upgrade libcrypto3 libssl3

# 4. Implementar CIS Docker Benchmark 4.1: Crear usuario no root

RUN addgroup -S appgroup && adduser -S appuser -G appgroup

# 5. Configurar variables de entorno para puerto 8080
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080

# 6. Copiar archivos con el dueño correcto (appuser)
COPY --from=build /app/publish .

# 7. Cambiar al usuario no privilegiado
USER appuser

# 8. Agregar HEALTHCHECK (Vital para detectar 'zombie containers')
HEALTHCHECK --interval=30s --timeout=5s --start-period=5s --retries=3 \
  CMD wget --no-verbose --tries=1 --spider http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "ServiceDemo.API.dll"]