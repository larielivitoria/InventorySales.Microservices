using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Estoque.Domain.Entities;
using Estoque.Domain.Interfaces;
using Estoque.Infrastructure.Db;
using Microsoft.EntityFrameworkCore;

namespace Estoque.Infrastructure.Repositorys
{
    public class ProdutoRepository : IProdutoRepository
    {
        private readonly DbEstoqueContext _context;
        public ProdutoRepository(DbEstoqueContext context)
        {
            _context = context;
        }

        public async Task<Produto> CriarProdutoAsync(Produto produto)
        {
            await _context.Produtos.AddAsync(produto);
            return produto;
        }

        public Produto AtualizarProduto(Produto produto)
        {
            _context.Produtos.Update(produto);
            return produto;
        }

        public async Task<Produto> ListarPorIdAsync(int id)
        {
            var listar = await _context.Produtos.FindAsync(id);
            if (listar == null)
            {
                throw new Exception($"Produto de Id {id} não encontrado.");
            }
            return listar;
        }

        public async Task<ICollection<Produto>> ListarTodosAsync()
        {
            var listar = await _context.Produtos.ToListAsync();
            return listar;
        }

        public async Task RemoverProdutoAsync(int id)
        {
            var remover = await _context.Produtos.FindAsync(id);
            if (remover == null)
            {
                throw new Exception($"Produto de Id {id} não encontrado.");
            }
            _context.Produtos.Remove(remover);
        }

        public async Task SalvarAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task DiminuirEstoqueAsync(int produtoId, int quantidade)
        {
            var produto = await _context.Produtos.FindAsync(produtoId);

            if (produto == null)
            {
                throw new KeyNotFoundException($"Produto {produtoId} não encontrado.");
            }

            if (produto.QuantidadeEstoque < quantidade)
            {
                throw new InvalidOperationException($"Não há estoque suficiente para o produto {produtoId}.");
            }

            produto.QuantidadeEstoque -= quantidade;

            _context.Produtos.Update(produto);
            await _context.SaveChangesAsync();
        }
    }
}