# Etapa de construcción: compila y publica la aplicación
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia el archivo de proyecto y restaura dependencias
COPY ["CarWashBackend.csproj", "./"]
RUN dotnet restore "CarWashBackend.csproj"

# Copia el resto del código y publica la aplicación
COPY . .
RUN dotnet publish "CarWashBackend.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Etapa final: se usa el SDK para disponer de 'dotnet ef'
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS final
WORKDIR /app

# Copia la aplicación publicada y el archivo .csproj (necesario para las migraciones)
COPY --from=build /app/publish .
COPY --from=build /src/CarWashBackend.csproj .

# Instala la herramienta global 'dotnet-ef'
RUN dotnet tool install --global dotnet-ef
ENV PATH="${PATH}:/root/.dotnet/tools"

# Expone el puerto 80
EXPOSE 80

# ENTRYPOINT que:
#  1. Espera a que MySQL esté disponible (usando bash y /dev/tcp)
#  2. Ejecuta las migraciones (dotnet ef database update)
#  3. Inicia la aplicación
ENTRYPOINT ["sh", "-c", "echo 'Esperando a que MySQL esté disponible...'; while ! bash -c 'echo > /dev/tcp/mysql/3306' 2>/dev/null; do sleep 1; done; echo 'MySQL está listo'; dotnet ef database update --project CarWashBackend.csproj && dotnet CarWashBackend.dll"]
