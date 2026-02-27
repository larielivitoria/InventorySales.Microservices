using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Shared.Interfaces;
using Venda.Domain.Interfaces;
using Venda.Infrastructure.Messaging;
using Venda.Infrastructure.Repositorys;

namespace Venda.IoC
{
    public static class DependencyInjectionsInfrastructure
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddScoped<IPedidoRepository, PedidoRepository>();
            services.AddScoped<IPedidoPublisher, PedidoPublisher>();
            return services;
        }
    }
}