using Core.SaaS.Inventory.Application.Interfaces;
using Core.SaaS.Inventory.API.Services;
using Microsoft.EntityFrameworkCore;
using Core.SaaS.Inventory.Infrastructure.Data;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// =========================================================
// FASE 1: REGISTRO DE SERVICIOS (El Contenedor)
// Todo lo que sea "builder.Services" debe ir estrictamente aquí
// =========================================================

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ITenantProvider, TenantProvider>();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// 1. Registramos Swagger con soporte para JWT
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Core.SaaS.Inventory API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Autorización JWT. Escribe 'Bearer' [espacio] y luego tu token.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } },
            new string[] {}
        }
    });
});

// 2. Registramos el motor de Autenticación JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "Core.SaaS.Inventory.Local",
            ValidAudience = "Core.SaaS.Inventory.Local",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("LaboratorioDev_SuperClaveSecreta_ParaDesarrollo_2026_!#"))
        };
    });

// =========================================================
// FRONTERA DE CONSTRUCCIÓN: El contenedor se sella aquí
// =========================================================

// Registramos MediatR buscando los casos de uso en la capa de Application
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Core.SaaS.Inventory.Application.Interfaces.ITenantProvider).Assembly));

var app = builder.Build();

// =========================================================
// FASE 2: PIPELINE HTTP (Manejo de peticiones)
// Todo lo que sea "app.Use..." debe ir estrictamente aquí
// =========================================================

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// El orden aquí es vital: Primero validas identidad, luego permisos
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();