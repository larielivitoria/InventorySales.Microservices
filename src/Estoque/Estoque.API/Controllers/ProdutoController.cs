using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Estoque.Application.DTOs;
using Estoque.Application.Interfaces;
using Estoque.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Estoque.API.Controllers
{
    [ApiController]
    [Route("[Controller]")]
    [Tags("Microsserviço de Estoque")]
    public class ProdutoController : ControllerBase
    {
        private readonly IProdutoService _produtoService;
        public ProdutoController(IProdutoService produtoService)
        {
            _produtoService = produtoService;
        }

        [HttpGet]
        public async Task<IActionResult> ListarTodos()
        {
            var todos = await _produtoService.ListarTodosAsync();
            return Ok(todos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ListarPorId(int id)
        {
            var listar = await _produtoService.ListarPorIdAsync(id);
            return Ok(listar);
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Gerente, Estoquista")]
        public async Task<IActionResult> CriarProduto([FromBody] ProdutoDTO produtoDTO)
        {
            var novo = await _produtoService.CriarProdutoAsync(produtoDTO);
            return CreatedAtAction(nameof(ListarPorId), new { id = novo.ProdutoId }, novo);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin, Gerente")]
        public async Task<IActionResult> AtualizarProduto(int id, [FromBody] AtualizaEstoqueDTO atualizaEstoqueDTO)
        {
            var produto = await _produtoService.AtualizarProdutoAsync(id, atualizaEstoqueDTO);
            return Ok(produto);
        }

        [HttpDelete]
        [Authorize(Roles = "Admin, Gerente")]
        public async Task<IActionResult> RemoverProduto(int id)
        {
            await _produtoService.RemoverProdutoAsync(id);
            return NoContent();
        }
    }
}