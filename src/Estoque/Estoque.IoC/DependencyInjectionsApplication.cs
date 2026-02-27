using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Estoque.Application.Handlers;
using Estoque.Application.Interfaces;
using Estoque.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Estoque.IoC
{
    public static class DependencyInjectionsApplication
    {
        public static IServiceCollection AddApplications(this IServiceCollection services)
        {
            services.AddScoped<IProdutoService, ProdutoService>();
            services.AddScoped<VerificarEstoqueHandler>();
            return services;
        }
    }
}