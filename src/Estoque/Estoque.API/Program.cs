using Estoque.Application.Handlers;
using Estoque.Domain.Interfaces;
using Estoque.Infrastructure.Db;
using Estoque.Infrastructure.Messaging;
using Estoque.Infrastructure.Repositorys;
using Estoque.IoC;
using Microsoft.EntityFrameworkCore;
using Shared.Messaging.RPC;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<DbEstoqueContext>(Options =>
    Options.UseSqlServer(builder.Configuration.GetConnectionString("ConexaoPadrao")));

builder.Services.AddApplications();
builder.Services.AddInfrastructure();

builder.Services.AddSingleton<IRabbitMQRequestBus, RabbitMQRequestBus>();
builder.Services.AddSingleton<PedidoCriadoConsumer>();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

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

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
