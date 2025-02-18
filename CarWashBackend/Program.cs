using CarWashBackend.Data;
using CarWashBackend.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Globalization;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Cargar las variables de entorno (puedes dejarlo para otros valores si lo necesitas)
builder.Configuration.AddEnvironmentVariables();

// Configuración de Kestrel para escuchar en el puerto 80
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(80);  // Kestrel escuchará en el puerto 80
});

// Obtener las variables de entorno para la base de datos
//var mysqlHost = Environment.GetEnvironmentVariable("MYSQL_HOST");
//var mysqlDatabase = Environment.GetEnvironmentVariable("MYSQL_DATABASE");
//var mysqlUser = Environment.GetEnvironmentVariable("MYSQL_USER");
//var mysqlPassword = Environment.GetEnvironmentVariable("MYSQL_PASSWORD");
//var connectionString = $"server={mysqlHost};database={mysqlDatabase};uid={mysqlUser};pwd={mysqlPassword}";

// Definir las credenciales de la base de datos de forma fija
var mysqlHost = "localhost";
var mysqlDatabase = "Carwash_DB";
var mysqlUser = "root";
var mysqlPassword = "P@ssWord.123";
var connectionString = $"server={mysqlHost};port=3306;database={mysqlDatabase};uid={mysqlUser};pwd={mysqlPassword}";
// Construir el ConnectionString para MySQL (incluyendo el puerto 3306)

// Configurar la zona horaria a Honduras
var hondurasTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central America Standard Time");
TimeZoneInfo.ClearCachedData(); // Limpiar caché de zona horaria por si acaso
TimeZoneInfo localTimeZone = TimeZoneInfo.Local;

Console.WriteLine($"Zona horaria del sistema: {localTimeZone.Id}");
Console.WriteLine($"Zona horaria esperada: {hondurasTimeZone.Id}");

// Configurar la cultura predeterminada
CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("es-HN");
CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("es-HN");

// Agregar servicios al contenedor
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configurar el DbContext para MySQL
builder.Services.AddDbContext<CarwashContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
);

// Configurar autenticación JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Crear un scope para ejecutar las migraciones pendientes y el seeder
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<CarwashContext>();

    try
    {
        // Ejecutar las migraciones pendientes en la base de datos
        Console.WriteLine("Aplicando migraciones...");
        context.Database.Migrate();
        Console.WriteLine("Migraciones aplicadas correctamente.");

        // Ejecutar el seeder para poblar datos (si es necesario)
        var seeder = new Seeder(context, true);
        seeder.Seed();
        Console.WriteLine("Seeding completo.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error al aplicar migraciones o al ejecutar el seeder: {ex.Message}");
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

// Ruta raíz que responde con un mensaje y la fecha actual en zona horaria de Honduras
app.MapGet("/", () =>
{
    // Obtener la zona horaria de Honduras
    var utcNow = DateTime.UtcNow;
    var hondurasTime = TimeZoneInfo.ConvertTimeFromUtc(utcNow, hondurasTimeZone);

    return Results.Ok(new
    {
        message = "API CarWash levantada correctamente",
        utcDate = utcNow.ToString("yyyy-MM-ddTHH:mm:ss"),
        localDate = hondurasTime.ToString("yyyy-MM-ddTHH:mm:ss")
    });
});

app.MapControllers();

app.Run();
