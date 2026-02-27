using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Venda.Application.Interfaces;
using Venda.Application.Services;

namespace Venda.IoC
{
    public static class DependencyInjectionsApplication
    {
        public static IServiceCollection AddApplications(this IServiceCollection services)
        {
            services.AddScoped<IPedidoService, PedidoService>();
            return services;
        }
    }
}