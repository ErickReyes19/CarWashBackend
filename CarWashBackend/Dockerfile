# Etapa de construcción
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia el archivo de proyecto y restaura dependencias
COPY ["CarWashBackend.csproj", "./"]
RUN dotnet restore "CarWashBackend.csproj"

# Copia el resto del código y publica la aplicación
COPY . .
RUN dotnet publish "CarWashBackend.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Etapa final: Usamos el SDK para tener acceso a dotnet-ef
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS final
WORKDIR /app

# Copia la aplicación publicada y el archivo .csproj (necesario para migraciones)
COPY --from=build /app/publish .
COPY --from=build /src/CarWashBackend.csproj .

# Instala la herramienta global dotnet-ef
RUN dotnet tool install --global dotnet-ef
ENV PATH="${PATH}:/root/.dotnet/tools"

EXPOSE 80

# Comando por defecto (se usará en el servicio "app")
CMD ["dotnet", "CarWashBackend.dll"]
