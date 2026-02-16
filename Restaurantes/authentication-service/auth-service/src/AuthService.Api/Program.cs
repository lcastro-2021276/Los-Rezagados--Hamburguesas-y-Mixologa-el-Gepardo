using AuthService.Application.Interfaces;
using AuthService.Application.Services;
using AuthService.Domain.Interfaces;
using AuthService.Persistence.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.IdentityModel.Tokens.Jwt;

var builder = WebApplication.CreateBuilder(args);

// ===============================
//  Servicios básicos
// ===============================
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// ===============================
//  Configuración Swagger
// ===============================
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Restaurant Authentication API",
        Version = "v1",
        Description = "Servicio de autenticación para el Sistema de Gestión de Restaurantes"
    });

    var jwtSecurityScheme = new OpenApiSecurityScheme
    {
        Scheme = "bearer",
        BearerFormat = "JWT",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Description = "Ingrese el token en formato: Bearer {su_token}",

        Reference = new OpenApiReference
        {
            Id = "Bearer",
            Type = ReferenceType.SecurityScheme
        }
    };

    options.AddSecurityDefinition("Bearer", jwtSecurityScheme);

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { jwtSecurityScheme, Array.Empty<string>() }
    });
});

// ===============================
//  Inyección de Dependencias
// ===============================

// Repositorio (temporal en memoria para pruebas)
builder.Services.AddScoped<IUserRepository, InMemoryUserRepository>();

// Servicio JWT
builder.Services.AddScoped<IJwtService>(provider =>
{
    var config = builder.Configuration.GetSection("Jwt");

    var key = config["Key"] 
        ?? throw new InvalidOperationException("Jwt:Key no está configurado");

    var issuer = config["Issuer"] 
        ?? throw new InvalidOperationException("Jwt:Issuer no está configurado");

    var audience = config["Audience"] 
        ?? throw new InvalidOperationException("Jwt:Audience no está configurado");

    var expires = int.TryParse(config["ExpiresMinutes"], out int minutes)
        ? minutes
        : 60;

    return new JwtService(key, issuer, audience, expires);
});

// Servicio de autenticación
builder.Services.AddScoped<IAuthService, AuthService.Application.Services.AuthService>();

// ===============================
// Servicio JWT
// ===============================
builder.Services.AddScoped<IJwtService>(provider =>
{
    var config = builder.Configuration.GetSection("Jwt");

    var key = config["Key"] 
        ?? throw new InvalidOperationException("Jwt:Key no está configurado");

    var issuer = config["Issuer"] 
        ?? throw new InvalidOperationException("Jwt:Issuer no está configurado");

    var audience = config["Audience"] 
        ?? throw new InvalidOperationException("Jwt:Audience no está configurado");

    var expires = int.TryParse(config["ExpiresMinutes"], out int minutes)
        ? minutes
        : 60;

    return new JwtService(key, issuer, audience, expires);
});


// ===============================
//  Middleware Pipeline
// ===============================
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.DocumentTitle = "Restaurant Auth API Docs";
    });
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
