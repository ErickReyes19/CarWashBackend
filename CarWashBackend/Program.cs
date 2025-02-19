using AspNetCoreRateLimit;
using CarWashBackend.Data;
using CarWashBackend.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Globalization;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Cargar las variables de entorno
builder.Configuration.AddEnvironmentVariables();

// Configuración de la base de datos y cultura
var mysqlHost = Environment.GetEnvironmentVariable("MYSQL_HOST");
var mysqlDatabase = Environment.GetEnvironmentVariable("MYSQL_DATABASE");
var mysqlUser = Environment.GetEnvironmentVariable("MYSQL_USER");
var mysqlPassword = Environment.GetEnvironmentVariable("MYSQL_PASSWORD");
var connectionString = $"server={mysqlHost};database={mysqlDatabase};uid={mysqlUser};pwd={mysqlPassword}";

// Configuración de la zona horaria y cultura
var hondurasTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central America Standard Time");
TimeZoneInfo.ClearCachedData();
CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("es-HN");
CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("es-HN");

// Agregar servicios al contenedor
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configurar DbContext para MySQL
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

// Configuración de rate limiting
builder.Services.AddInMemoryRateLimiting();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

builder.Services.AddMemoryCache();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Crear un scope para ejecutar las migraciones pendientes y el seeder
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<CarwashContext>();

    try
    {
        // Ejecutar las migraciones pendientes
        Console.WriteLine("Aplicando migraciones...");
        context.Database.Migrate();
        Console.WriteLine("Migraciones aplicadas correctamente.");

        // Ejecutar el seeder para poblar datos
        var seeder = new Seeder(context, true);
        seeder.Seed();
        Console.WriteLine("Seeding completo.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error al aplicar migraciones o al ejecutar el seeder: {ex.Message}");
        Environment.Exit(1); // Termina la aplicación si falla la migración
    }
}

// Configurar Swagger para desarrollo
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Aplicar Rate Limiting Middleware
app.UseIpRateLimiting(); // Aplica el rate limiting en todas las peticiones

// Habilitar autenticación y autorización
app.UseAuthentication();
app.UseAuthorization();

// Ruta raíz
app.MapGet("/", () =>
{
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
