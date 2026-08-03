using Estoque.Application.Handlers;
using Estoque.Domain.Interfaces;
using Estoque.Infrastructure.Db;
using Estoque.Infrastructure.Messaging;
using Estoque.Infrastructure.Repositorys;
using Estoque.IoC;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Shared.Messaging.RPC;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<DbEstoqueContext>(Options =>
    Options.UseSqlServer(builder.Configuration.GetConnectionString("ConexaoPadrao")));

// Configuração do JWT
var jwtSecretKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("Chave Secreta do JWT não configurada!");

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
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecretKey)),

        ValidateIssuer = false,
        ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "ApiGatewayAuth",

        ValidateAudience = false,
        ValidAudience = builder.Configuration["Jwt:Audience"] ?? "Microservices",

        ClockSkew = TimeSpan.Zero,

        NameClaimType = System.Security.Claims.ClaimTypes.NameIdentifier,
        RoleClaimType = System.Security.Claims.ClaimTypes.Role
    };
});

builder.Services.AddApplications();
builder.Services.AddInfrastructure();

builder.Services.AddSingleton<IRabbitMQRequestBus, RabbitMQRequestBus>();
builder.Services.AddSingleton<PedidoCriadoConsumer>();

builder.Services.AddControllers();

builder.Services.AddAuthorization();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "ApiGateway", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Digite 'Bearer ' seguido do token JWT abaixo.\n\n Exemplo: 'Bearer eyJhbGciOi...'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

app.UseRouting();

var consumer = app.Services.GetRequiredService<PedidoCriadoConsumer>();
var bus = app.Services.GetRequiredService<IRabbitMQRequestBus>();

bus.RespondAsync<VerificarEstoqueRequest, VerificarEstoqueResponse>(async request =>
{
    using var scope = app.Services.CreateScope();
    var handler = scope.ServiceProvider.GetRequiredService<VerificarEstoqueHandler>();

    return await handler.Handle(request);
});

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
