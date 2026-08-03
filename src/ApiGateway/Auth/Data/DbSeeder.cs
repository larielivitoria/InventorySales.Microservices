using ApiGateway.Auth.Models;
using Microsoft.EntityFrameworkCore;
namespace ApiGateway.Auth.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAdminAsync(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AuthDbContext>();

            // Garante q o banco e as migrations estejam aplicados
            await context.Database.MigrateAsync();

            var emailAdmin = configuration["SeedUser:Email"];
            var senhaAdmin = configuration["SeedUser:Senha"];
            var roleAdmin = configuration["SeedUser:Role"] ?? "Admin";

            if(string.IsNullOrEmpty(emailAdmin) || string.IsNullOrEmpty(senhaAdmin))
            {
                return; // sai do seed se as variáveis não estiverem configuradas
            }

            // Verifica se o Admin já existe
            var adminExiste = await context.Usuarios.AnyAsync(u => u.Email == emailAdmin);

            if (!adminExiste)
            {
                var admin = new Usuario(emailAdmin, senhaAdmin, roleAdmin);

                await context.Usuarios.AddAsync(admin);
                await context.SaveChangesAsync();
            }
        }
    }
}
