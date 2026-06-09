using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SoulFoodAiBack.Data;
using System.Text;

// Inicializamos el constructor de la aplicación web
var builder = WebApplication.CreateBuilder(args);

// 1. Añadimos el soporte para los Controladores (nuestros endpoints)
builder.Services.AddControllers();

// 2. Configuración de Swagger (Documentación automática de la API)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    // Le indicamos a Swagger dónde está el archivo XML con todos los comentarios (///) que hemos hecho en los Controladores y Modelos
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = System.IO.Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});

// 3. Configuración de la Base de Datos (Entity Framework Core conectando a SQL Server)
builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlServerOptionsAction: sqlOptions =>
        {
            // Resiliencia de BD: si la conexión falla (ej. microcorte en la nube), reintenta automáticamente hasta 5 veces
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(30),
                errorNumbersToAdd: null);
        }));

// 4. Configuración de CORS (Permisos de acceso cruzado)
// Es vital para que tu frontend en Angular no sea bloqueado por el navegador al hacer peticiones a este backend
builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitirAngular", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// 5. Configuración de Seguridad y Autenticación (Tokens JWT)
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true, // Comprueba que el token no haya caducado
            ValidateIssuerSigningKey = true, // Verifica que el token fue firmado por nosotros
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            // Convierte la clave secreta del appsettings.json en bytes para firmar/verificar los tokens
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });

// 6. Inyección de Dependencias de servicios propios
// AddScoped crea una instancia nueva del servicio (AuthService) por cada petición HTTP que nos llegue
builder.Services.AddScoped<SoulFoodAiBack.Services.AuthService>();

// --- CONSTRUCCIÓN DE LA APP ---
var app = builder.Build();

// 7. Configuración del Pipeline de Middlewares (El orden aquí es MUY importante)

// Activar la interfaz web de Swagger
app.UseSwagger();
app.UseSwaggerUI();

// Redirigir todo el tráfico HTTP hacia HTTPS por seguridad
app.UseHttpsRedirection();

// Aplicar las reglas de CORS antes de llegar a la autenticación para que Angular no dé fallo de red
app.UseCors("PermitirAngular");

// Verificar quién es el usuario (Autenticación) y qué permisos tiene (Autorización)
app.UseAuthentication();
app.UseAuthorization();

// Mapear las rutas hacia los Controladores
app.MapControllers();

// Arrancar el servidor
app.Run();