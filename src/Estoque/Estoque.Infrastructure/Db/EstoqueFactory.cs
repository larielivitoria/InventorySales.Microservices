using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Estoque.Infrastructure.Db
{
    public class EstoqueFactory : IDesignTimeDbContextFactory<DbEstoqueContext>
    {
        public DbEstoqueContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<DbEstoqueContext>();

            optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB; Initial Catalog=Estoque;Integrated Security=True;TrustServerCertificate=True;");

            return new DbEstoqueContext(optionsBuilder.Options);
        }
    }
}