# Usar la imagen base de .NET SDK 8 para la construcción
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copiar el archivo .csproj y restaurar dependencias
COPY ["CarWashBackend.csproj", "."]
RUN dotnet restore "CarWashBackend.csproj"

# Copiar todo el código fuente del backend
COPY . .

# Construir el proyecto en modo Release
RUN dotnet build "CarWashBackend.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publicar la aplicación en modo Release
FROM build AS publish
RUN dotnet publish "CarWashBackend.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Establecer la imagen base para el runtime (también con .NET 8)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Instalar dependencias, wget, y MySQL client desde otro repositorio
RUN apt-get update && apt-get install -y \
    gnupg \
    wget \
    lsb-release && \
    wget https://dev.mysql.com/get/mysql-apt-config_0.8.17-1_all.deb && \
    dpkg -i mysql-apt-config_0.8.17-1_all.deb && \
    apt-get update && \
    apt-get install -y mysql-client

# Copiar los archivos publicados desde la etapa de publicación
COPY --from=publish /app/publish .

# Exponer el puerto 80 para la aplicación
EXPOSE 80

# Establecer el comando por defecto para ejecutar la aplicación
ENTRYPOINT ["dotnet", "CarWashBackend.dll"]
