using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Estoque.Domain.Entities;

namespace Estoque.Domain.Interfaces
{
    public interface IProdutoRepository
    {
        public Task<Produto> CriarProdutoAsync(Produto produto);
        public Produto AtualizarProduto(Produto produto);
        public Task<ICollection<Produto>> ListarTodosAsync();
        public Task<Produto> ListarPorIdAsync(int id);
        public Task RemoverProdutoAsync(int id);
        public Task DiminuirEstoqueAsync(int produtoId, int quantidade);
        public Task SalvarAsync();
    }
}