using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Estoque.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Estoque.Infrastructure.Db
{
    public class DbEstoqueContext : DbContext
    {
        public DbEstoqueContext(DbContextOptions<DbEstoqueContext> options) : base(options)
        {

        }
        
        public DbSet<Produto> Produtos { get; set; }
    }
}