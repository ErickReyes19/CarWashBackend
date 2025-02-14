# Etapa de construcción
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

# Copiar los archivos necesarios para la construcción
COPY ["CarWashBackend/CarWashBackend.csproj", "CarWashBackend/"]
RUN dotnet restore "CarWashBackend/CarWashBackend.csproj"

COPY . .
WORKDIR "/src/CarWashBackend"
RUN dotnet publish "CarWashBackend.csproj" -c Release -o /app

# Etapa final (con solo el runtime)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Copiar los archivos de la etapa de construcción
COPY --from=build /app .

# Ejecutar las migraciones al inicio del contenedor
RUN dotnet ef database update --connection "Server=mysql;Database=Carwash_DB;User=root;Password=P@ssWord.123"

# Exponer el puerto
EXPOSE 80

# Iniciar la aplicación después de aplicar las migraciones
ENTRYPOINT ["dotnet", "CarWashBackend.dll"]
