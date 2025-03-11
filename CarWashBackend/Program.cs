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

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(8081);  // Kestrel escuchará en el puerto 8081
});

// Configuración de la base de datos y cultura
var mysqlHost = Environment.GetEnvironmentVariable("MYSQL_HOST");
var mysqlDatabase = Environment.GetEnvironmentVariable("MYSQL_DATABASE");
var mysqlUser = Environment.GetEnvironmentVariable("MYSQL_USER");
var mysqlPassword = Environment.GetEnvironmentVariable("MYSQL_PASSWORD");
var connectionString = $"server={mysqlHost};database={mysqlDatabase};uid={mysqlUser};pwd={mysqlPassword}";

// Definir las credenciales de la base de datos de forma fija
//var mysqlHost = "localhost";
//var mysqlDatabase = "Carwash_DB";
//var mysqlUser = "root";
//var mysqlPassword = "P@ssWord.123";
//var connectionString = $"server={mysqlHost};port=3306;database={mysqlDatabase};uid={mysqlUser};pwd={mysqlPassword}";
//Construir el ConnectionString para MySQL (incluyendo el puerto 3306)

// Configuración de la zona horaria y cultura
var hondurasTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central America Standard Time");
TimeZoneInfo.ClearCachedData();
CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("es-HN");
CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("es-HN");


// Agregar servicios al contenedor
builder.Services.AddControllers();
builder.Services.AddSignalR();
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
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", policy =>
    {
        policy.WithOrigins("https://carwash-front-end.vercel.app") // Reemplaza con el origen de tu frontend
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials(); // Permite el uso de credenciales
    });
});

builder.Services.AddAuthorization();

// Configuración de rate limiting

// Configuración de rate limiting desde el appsettings.json
builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));  // Cargar las reglas de rate limiting desde el archivo de configuración
builder.Services.AddInMemoryRateLimiting();  // Usar almacenamiento en memoria para las reglas de rate limiting
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();  // Configuración del rate limit
builder.Services.AddMemoryCache();  // Habilitar almacenamiento en caché para los contadores de rate limiting
builder.Services.AddHttpContextAccessor(); // Acceso al contexto HTTP para evaluar las peticiones

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

// Configurar Swagger para desarrollo
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Aplicar Rate Limiting Middleware antes de la autenticación y autorización
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
app.UseCors("AllowSpecificOrigin");
app.MapControllers();
app.MapHub<OrderHub>("/orderHub");
app.Run();
