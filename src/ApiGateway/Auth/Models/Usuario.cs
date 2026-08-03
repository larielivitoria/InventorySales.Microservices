using Microsoft.AspNetCore.Identity;

namespace ApiGateway.Auth.Models
{
    public class Usuario
    {
        public Guid Id { get; private set; }
        public string Email { get; private set; } = null!;
        public string SenhaHash { get; private set; } = null!;
        public string Role { get; private set; } = null!;
        public DateTime CriadoEm { get; private set; }

        // construtor sem parâmetro para o EF Core
        private Usuario() { }

        // construtor público para criação controlada
        public Usuario(string email, string senhaPura, string role)
        {
            Id = Guid.NewGuid();
            Email = email;
            SenhaHash = HashSenha(senhaPura);
            Role = role;
            CriadoEm = DateTime.UtcNow;
        }

        private string HashSenha( string senhaPura)
        {
            var hasher = new PasswordHasher<Usuario>();
            return hasher.HashPassword(this, senhaPura);
        }

        public bool ValidarSenha(string senhaPura)
        {
            var hasher = new PasswordHasher<Usuario>();
            var resultado = hasher.VerifyHashedPassword(this, SenhaHash, senhaPura);

            return resultado != PasswordVerificationResult.Failed;
        }

        public void AlterarRole(string novaRole)
        {
            if(novaRole != "Estoquista" && novaRole != "Gerente")
            {
                throw new ArgumentException("Este Cargo não é válido.");
            }

            Role = novaRole;
        }
    }
}
