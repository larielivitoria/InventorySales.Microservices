using Microsoft.EntityFrameworkCore;
using Shared.Interfaces;
using Shared.Messaging.RPC;
using Venda.Application.Interfaces;
using Venda.Application.Services;
using Venda.Domain.Interfaces;
using Venda.Infrastructure.Db;
using Venda.Infrastructure.Messaging;
using Venda.Infrastructure.Repositorys;
using Venda.IoC;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<VendaContext>(Options =>
    Options.UseSqlServer(builder.Configuration.GetConnectionString("ConexaoPadrao")));

builder.Services.AddApplications();
builder.Services.AddInfrastructure();

builder.Services.AddSingleton<IRabbitMQRequestBus, RabbitMQRequestBus>();
builder.Services.AddSingleton<IPedidoPublisher, PedidoPublisher>();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
