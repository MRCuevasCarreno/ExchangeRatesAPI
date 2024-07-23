using ExchangeRatesAPI.Data;
using ExchangeRatesAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);
var origenesPermitidos = builder.Configuration.GetValue<string?>("origenesPermitidos");

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddHttpClient<FrankfurterService>();
builder.Services.AddScoped<FrankfurterService>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            // Configure JWT validation parameters
        };
    });


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
builder.Services.AddSwaggerGen();

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
