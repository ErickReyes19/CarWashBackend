using CarWashBackend.Data;
using CarWashBackend.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Cargar las variables de entorno
builder.Configuration.AddEnvironmentVariables();

// Configuración de Kestrel para escuchar en el puerto 80
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(80);  // Kestrel escuchará en el puerto 80
});

// Obtener las variables de entorno para la base de datos
var mysqlHost = Environment.GetEnvironmentVariable("MYSQL_HOST");
var mysqlDatabase = Environment.GetEnvironmentVariable("MYSQL_DATABASE");
var mysqlUser = Environment.GetEnvironmentVariable("MYSQL_USER");
var mysqlPassword = Environment.GetEnvironmentVariable("MYSQL_PASSWORD");

// Construir el ConnectionString para MySQL
var connectionString = $"server={mysqlHost};database={mysqlDatabase};uid={mysqlUser};pwd={mysqlPassword}";

// Agregar servicios al contenedor
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configurar el DbContext para MySQL
builder.Services.AddDbContext<CarwashContext>(options =>
    options.UseMySql(
        connectionString,
        ServerVersion.AutoDetect(connectionString))
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
    var hondurasTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central America Standard Time");

    // Obtener la hora actual UTC
    var utcNow = DateTime.UtcNow;

    // Convertir la hora UTC a la hora de Honduras
    var hondurasTime = TimeZoneInfo.ConvertTimeFromUtc(utcNow, hondurasTimeZone);

    // Responder con la hora convertida
    return Results.Ok(new
    {
        message = "API CarWash levantada correctamente",
        currentDate = hondurasTime.ToString("yyyy-MM-ddTHH:mm:ss")
    });
});

app.MapControllers();

app.Run();
