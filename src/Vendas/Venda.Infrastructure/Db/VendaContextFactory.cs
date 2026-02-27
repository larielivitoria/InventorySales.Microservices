using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Venda.Infrastructure.Db
{
    public class VendaContextFactory : IDesignTimeDbContextFactory<VendaContext>
    {
        public VendaContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<VendaContext>();

            optionsBuilder.UseSqlServer("Server=localhost\\sqlexpress; Initial Catalog=Vendas;Integrated Security=True;TrustServerCertificate=True;");

            return new VendaContext(optionsBuilder.Options);
        }
    }
}