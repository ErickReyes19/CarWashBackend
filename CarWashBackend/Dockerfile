# Etapa de construcción
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copiar y restaurar dependencias
COPY ["CarWashBackend.csproj", "./"]
RUN dotnet restore "CarWashBackend.csproj"

# Copiar el código fuente y construir
COPY . .
WORKDIR "/src"
RUN dotnet build "CarWashBackend.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publicar la aplicación
RUN dotnet publish "CarWashBackend.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Etapa final: Imagen optimizada (solo runtime)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Copiar los archivos publicados
COPY --from=build /app/publish .

# Copiar el script para esperar MySQL
COPY wait-for-it.sh /usr/local/bin/wait-for-it
RUN chmod +x /usr/local/bin/wait-for-it

# Exponer el puerto 80
EXPOSE 80

# Ejecutar migraciones y luego iniciar la app
ENTRYPOINT ["sh", "-c", "/usr/local/bin/wait-for-it mysql && dotnet ef database update && dotnet CarWashBackend.dll"]
