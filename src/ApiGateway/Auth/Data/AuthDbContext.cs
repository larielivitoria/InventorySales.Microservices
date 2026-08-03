using ApiGateway.Auth.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiGateway.Auth.Data
{
    public class AuthDbContext : DbContext
    {
        //construtor repassa a Connection String para o DbContext
        public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options)
        {

        }

        //criação da nossa tabela, baseada na classe Usuario, chamada Usuarios
        public DbSet<Usuario> Usuarios { get; private set; }
    }
}
