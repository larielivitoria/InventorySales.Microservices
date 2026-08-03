
using ApiGateway.Auth.Data;
using ApiGateway.Auth.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

//Config Ocelot
builder.Configuration.SetBasePath(builder.Environment.ContentRootPath)
                     .AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

//Config AuthDbContext
builder.Services.AddDbContext<AuthDbContext>(options =>
                 options.UseSqlServer(builder.Configuration.GetConnectionString("AuthConexao")));

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

// DI do TokenService
builder.Services.AddScoped<ITokenService, TokenService>();

builder.Services.AddControllers();

builder.Services.AddAuthorization();

builder.Services.AddEndpointsApiExplorer();

// Swagger do Gateway com suporte ao botão Authorize (cadeados)
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

builder.Services.AddOcelot(builder.Configuration);
builder.Services.AddSwaggerForOcelot(builder.Configuration, options =>
{
    options.GenerateDocsForGatewayItSelf = true;
});

var app = builder.Build();

// Executa o Seeder para garantir o banco atualizado e o usuário Gerente criado
await DbSeeder.SeedAdminAsync(app.Services, app.Configuration);

// Middlewares
app.UseHttpsRedirection();

app.UseRouting();

app.UseSwagger();

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(options =>
{
    options.MapControllers();
});

if (app.Environment.IsDevelopment())
{
    app.UseSwaggerForOcelotUI(ocelotOptions => //Configs do Ocelot
    {
        ocelotOptions.PathToSwaggerGenerator = "/swagger/docs";
    },
        // Configs nativas do SwaggerUI como 2º parâmetro = mantem o Token JWT salvo mesmo após atualizar a página
        swaggerUiOptions =>
        {
            swaggerUiOptions.EnablePersistAuthorization();
        });
}
await app.UseOcelot();

app.Run();

