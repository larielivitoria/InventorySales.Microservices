using ApiGateway.Auth.Models;

namespace ApiGateway.Auth.Services
{
    public interface ITokenService
    {
        public string GerarToken(Usuario usuario);
    }
}
