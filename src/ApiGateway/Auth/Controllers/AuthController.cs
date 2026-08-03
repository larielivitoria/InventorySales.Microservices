using ApiGateway.Auth.Data;
using ApiGateway.Auth.DTOs;
using ApiGateway.Auth.Models;
using ApiGateway.Auth.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiGateway.Auth.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthDbContext _context;
        private readonly ITokenService _tokenService;

        public AuthController(AuthDbContext context, ITokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login ([FromBody] LoginDTO loginDTO)
        {
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == loginDTO.Email);

            if (usuario == null)
            {
                return Unauthorized(new { mensagem = "E-mail ou Senha Inválidos." });
            }

            if (!usuario.ValidarSenha(loginDTO.Senha))
            {
                return Unauthorized(new { mensagem = "E-mail ou Senha Inválidos." });
            }

            var token = _tokenService.GerarToken(usuario);

            return Ok(new
            {
                token,
                email = usuario.Email,
                senha = usuario.SenhaHash
            });
        }

        [HttpPost("registrar")]
        public async Task<IActionResult> Registrar ([FromBody] RegistroDTO registroDTO)
        {
            var emailEmUso = await _context.Usuarios.AnyAsync(u => u.Email == registroDTO.Email);

            if (emailEmUso)
            {
                return BadRequest(new { mensagem = "Este E-mail já está Cadastrado." });
            }

            var novoUsuario = new Usuario(registroDTO.Email, registroDTO.Senha, role: "Cliente");

            await _context.Usuarios.AddAsync(novoUsuario);
            await _context.SaveChangesAsync();

            return StatusCode(201, new { mensagem = "Usuário Cadastrado com Sucesso." });
        }

        [HttpPost("CadastroFuncionario")]
        [Authorize(Roles = "Admin, Gerente")]
        public async Task<IActionResult> CadastroDeFuncionario([FromBody] FuncionarioDTO funcionarioDTO)
        {
            var emailEmUso = await _context.Usuarios.AnyAsync(u => u.Email == funcionarioDTO.Email);

            if (emailEmUso)
            {
                return BadRequest(new { mensagem = "Este E-mail já está Cadastrado." });
            }

            if(funcionarioDTO.Role != "Gerente" && funcionarioDTO.Role != "Estoquista")
            {
                return BadRequest(new { mensagem = "Este Cargo não é Válido." });
            }

            var novoFuncionario = new Usuario(funcionarioDTO.Email, funcionarioDTO.Senha, funcionarioDTO.Role);

            await _context.Usuarios.AddAsync(novoFuncionario);
            await _context.SaveChangesAsync();

            return StatusCode(201, new { mensagem = "Funcionário Cadastrado com Sucesso." });
        }

        [HttpPost("PromoverFuncionario")]
        [Authorize(Roles = "Admin, Gerente")]
        public async Task<IActionResult> PromoverFuncionario([FromBody] PromocaoDTO promocaoDTO)
        {
            var funcionarioExiste = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == promocaoDTO.Email);

            if(funcionarioExiste == null)
            {
                return BadRequest(new { mensagem = "Funcionário Não Cadastrado." });
            }

            try
            {
                funcionarioExiste.AlterarRole(promocaoDTO.Role);
            }
            catch(InvalidOperationException ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }

            await _context.SaveChangesAsync();

            return Ok(new { mensagem = "Funcionário Promovido com Sucesso." });
        }
    }
}
