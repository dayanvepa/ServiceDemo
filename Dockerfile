# ── Etapa 1: Build ──────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copiar archivos de solución y proyectos primero (para cachear restore)
COPY ServiceDemo.slnx ./
COPY src/ServiceDemo.Domain/ServiceDemo.Domain.csproj             src/ServiceDemo.Domain/
COPY src/ServiceDemo.Application/ServiceDemo.Application.csproj   src/ServiceDemo.Application/
COPY src/ServiceDemo.Infrastructure/ServiceDemo.Infrastructure.csproj src/ServiceDemo.Infrastructure/
COPY src/ServiceDemo.API/ServiceDemo.API.csproj                   src/ServiceDemo.API/

# Restore
RUN dotnet restore src/ServiceDemo.API/ServiceDemo.API.csproj

# Copiar todo el código fuente
COPY src/ src/

# Publicar (sin --no-restore para que descargue analyzers y herramientas faltantes)
RUN dotnet publish src/ServiceDemo.API/ServiceDemo.API.csproj \
    -c Release \
    -o /app/publish

# ── Etapa 2: Runtime ─────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "ServiceDemo.API.dll"]