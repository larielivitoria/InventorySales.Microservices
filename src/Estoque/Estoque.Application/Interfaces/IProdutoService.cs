using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Estoque.Application.DTOs;
using Estoque.Domain.Entities;

namespace Estoque.Application.Interfaces
{
    public interface IProdutoService
    {
        public Task<Produto> CriarProdutoAsync(ProdutoDTO produtoDTO);
        public Task<Produto> AtualizarProdutoAsync(int id, AtualizaEstoqueDTO atualizaEstoqueDTO);
        public Task<ICollection<Produto>> ListarTodosAsync();
        public Task<Produto> ListarPorIdAsync(int id);
        public Task RemoverProdutoAsync(int id);
    }
}