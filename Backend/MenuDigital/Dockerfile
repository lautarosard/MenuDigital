# Etapa 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar la solución
COPY MenuDigital.sln .

# Copiar todos los .csproj de cada proyecto
COPY Application/Application.csproj Application/
COPY Domain/Domain.csproj Domain/
COPY Infrastructure/Infrastructure.csproj Infrastructure/
COPY MenuDigital/MenuDigital.csproj MenuDigital/

# Restaurar dependencias de la solución
RUN dotnet restore MenuDigital.sln

# Copiar todo el código
COPY . .

# Compilar y publicar solo el proyecto de la API
WORKDIR /src/MenuDigital
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# Etapa 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "MenuDigital.dll"]
