using ExchangeRatesAPI.Data;
using ExchangeRatesAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var origenesPermitidos = builder.Configuration.GetValue<string?>("origenesPermitidos");

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddHttpClient<FrankfurterService>();
builder.Services.AddScoped<FrankfurterService>();

//Implementar un sistema de autenticación utilizando Json Web Tokens (JWT) para proteger los endpoints CRUD. OK
var key = Encoding.ASCII.GetBytes("ThisIsASecretKeyWith32Characters12345");
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

//Configurar caché en memoria para mejorar el rendimiento de las solicitudes GET. OK
builder.Services.AddMemoryCache();
builder.Services.AddResponseCaching();
builder.Services.AddCors(
    opciones => {
        opciones.AddDefaultPolicy(
            configuracion =>
            {
                configuracion.WithOrigins(origenesPermitidos).AllowAnyHeader().AllowAnyMethod();
            }
        );

        opciones.AddPolicy("libre",
            configuracion =>
            {
                configuracion.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
            }
        );
    });
builder.Services.AddOutputCache();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "ExchangeRatesAPI", Version = "v1" });

    // Configurar JWT Authentication para Swagger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Ingrese 'Bearer' [espacio] y luego su token en el campo de texto. \r\n\r\nEjemplo: \"Bearer 12345abcdef\""
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseOutputCache();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
