using AuthService.Application.Interfaces;
using AuthService.Application.Services;
using AuthService.Domain.Interfaces;
using AuthService.Persistence.Repositories;
using AuthService.Api.Middlewares;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// ====================
// Controllers & Swagger
// ====================
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

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
        Name = "Authorization",
        Description = "Ingrese el token en formato: Bearer {token}",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
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

// ====================
// JWT Configuration
// ====================
var jwtConfig = builder.Configuration.GetSection("Jwt");

var key = jwtConfig["Key"]
    ?? throw new InvalidOperationException("Jwt:Key no está configurado");
var issuer = jwtConfig["Issuer"]
    ?? throw new InvalidOperationException("Jwt:Issuer no está configurado");
var audience = jwtConfig["Audience"]
    ?? throw new InvalidOperationException("Jwt:Audience no está configurado");

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false; // ✅ LOCAL
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(key)),
            ClockSkew = TimeSpan.Zero
        };
    });

// ====================
// Authorization
// ====================
builder.Services.AddAuthorization();

// ====================
// Rate Limiting
// ====================
builder.Services.AddRateLimiter(options =>
{
    options.AddPolicy("ApiPolicy", context =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "anonymous",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 10,
                Window = TimeSpan.FromSeconds(60),
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 2
            }));
});

// ====================
// Dependency Injection
// ====================
builder.Services.AddScoped<IUserRepository, InMemoryUserRepository>();

builder.Services.AddScoped<IJwtService>(_ =>
{
    var expires = int.TryParse(jwtConfig["ExpiresMinutes"], out int minutes)
        ? minutes
        : 60;

    return new JwtService(key, issuer, audience, expires);
});

builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IAuthService, AuthService.Application.Services.AuthService>();

// ====================
// App Pipeline
// ====================
var app = builder.Build();

// 🔥 Swagger SIEMPRE disponible en local
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Restaurant Auth API v1");
    c.RoutePrefix = "swagger";
});

// ❌ NO HTTPS en local
// app.UseHttpsRedirection();

app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseRateLimiter();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();