using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Venda.Application.DTOs;
using Venda.Application.Interfaces;
using Venda.Domain.Entities;

namespace Venda.API.Controllers
{
    [ApiController]
    [Route("[Controller]")]
    [Tags("Microsserviço de Vendas")]
    public class PedidoController : ControllerBase
    {
        private readonly IPedidoService _pedidoService;

        public PedidoController(IPedidoService pedidoService)
        {
            _pedidoService = pedidoService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin, Gerente, Cliente")]
        public async Task<IActionResult> ListarTodos()
        {
            var todos = await _pedidoService.ListarTodosAsync();
            return Ok(todos);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin, Gerente, Cliente")]
        public async Task<IActionResult> ListarPorId(int id)
        {
            var listar = await _pedidoService.ListarPorIdAsync(id);
            return Ok(listar);
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Gerente, Cliente")]
        public async Task<IActionResult> CriarPedido([FromBody] PedidoDTO pedidoDTO)
        {
            var pedido = await _pedidoService.CriarPedidoAsync(pedidoDTO);
            return CreatedAtAction(nameof(ListarPorId), new { id = pedido.PedidoId }, pedido);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin, Gerente")]
        public async Task<IActionResult> AtualizarPedido(int id, [FromBody] AtualizarStatusDTO atualizarStatusDTO)
        {
            var pedido = await _pedidoService.AtualizarPedidoAsync(id, atualizarStatusDTO);
            return Ok(pedido);
        }

        [HttpDelete]
        [Authorize(Roles = "Admin, Gerente")]
        public async Task<IActionResult> RemoverPedido(int id)
        {
            await _pedidoService.RemoverPedidoAsync(id);
            return NoContent();
        }
    }
}