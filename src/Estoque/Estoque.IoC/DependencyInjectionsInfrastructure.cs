using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Estoque.Domain.Interfaces;
using Estoque.Infrastructure.Repositorys;
using Microsoft.Extensions.DependencyInjection;

namespace Estoque.IoC
{
    public static class DependencyInjectionsInfrastructure
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddScoped<IProdutoRepository, ProdutoRepository>();
            return services;
        }
    }
}