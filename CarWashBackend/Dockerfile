# Usar la imagen base de .NET SDK 8 para la construcción
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copiar el archivo .csproj y restaurar dependencias
COPY ["CarWashBackend/CarWashBackend.csproj", "CarWashBackend/"]
RUN dotnet restore "CarWashBackend/CarWashBackend.csproj"

# Copiar el resto de los archivos del proyecto
COPY . .

# Establecer el directorio de trabajo en la carpeta donde está el proyecto
WORKDIR "/src/CarWashBackend"

# Construir el proyecto en modo Release
RUN dotnet build "CarWashBackend.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publicar la aplicación en modo Release
FROM build AS publish
RUN dotnet publish "CarWashBackend.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Verificar el contenido de /app/publish para asegurarse de que la DLL está allí
RUN ls -l /app/publish

# Establecer la imagen base para el runtime (también con .NET 8)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Copiar los archivos publicados desde la etapa de publicación
COPY --from=publish /app/publish .

# Exponer el puerto 80 para la aplicación
EXPOSE 80

# Establecer el comando por defecto para ejecutar la aplicación
ENTRYPOINT ["dotnet", "CarWashBackend.dll"]
