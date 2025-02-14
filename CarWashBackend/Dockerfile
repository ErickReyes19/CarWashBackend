# Etapa de construcción
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copiar solo archivos de proyecto y restaurar dependencias
COPY CarWashBackend.csproj .

RUN dotnet restore "CarWashBackend/CarWashBackend.csproj"

# Copiar el resto de los archivos
COPY . .
WORKDIR "/src/CarWashBackend"

# Compilar y publicar en una sola etapa
RUN dotnet publish "CarWashBackend.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Imagen final basada en runtime de .NET
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Copiar archivos publicados
COPY --from=build /app/publish .

# Exponer el puerto 80
EXPOSE 80

# Ejecutar migraciones antes de iniciar la API
ENTRYPOINT ["sh", "-c", "dotnet CarWashBackend.dll"]
