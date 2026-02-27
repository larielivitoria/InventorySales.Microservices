using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Estoque.Application.DTOs;
using Estoque.Application.Interfaces;
using Estoque.Domain.Entities;
using Estoque.Domain.Interfaces;


namespace Estoque.Application.Services
{
    public class ProdutoService : IProdutoService
    {
        private readonly IProdutoRepository _produtoRepository;
        public ProdutoService(IProdutoRepository produtoRepository)
        {
            _produtoRepository = produtoRepository;
        }

        public async Task<Produto> AtualizarProdutoAsync(int id, AtualizaEstoqueDTO atualizaEstoqueDTO)
        {
            var produto = await _produtoRepository.ListarPorIdAsync(id);

            if (atualizaEstoqueDTO.QuantidadeEstoque < 0)
            {
                throw new Exception($"Quantidade em estoque deve ser maior ou igual a zero.");
            }

            produto.QuantidadeEstoque = atualizaEstoqueDTO.QuantidadeEstoque;

            _produtoRepository.AtualizarProduto(produto);
            await _produtoRepository.SalvarAsync();

            return produto;
        }

        public async Task<Produto> CriarProdutoAsync(ProdutoDTO produtoDTO)
        {
            if (string.IsNullOrWhiteSpace(produtoDTO.Nome))
            {
                throw new Exception("Nome não pode estar vazio.");
            }

            if (string.IsNullOrWhiteSpace(produtoDTO.Descricao))
            {
                produtoDTO.Descricao = "Descrição não informada.";
            }

            if (produtoDTO.Preco <= 0)
            {
                throw new Exception("Preço deve ser maior que zero.");
            }

            if (produtoDTO.QuantidadeEstoque < 0)
            {
                throw new Exception("Quantidade em Estoque deve ser maior ou igual a zero.");
            }
            var prod = new Produto(produtoDTO.Nome, produtoDTO.Descricao, produtoDTO.Preco, produtoDTO.QuantidadeEstoque);

            await _produtoRepository.CriarProdutoAsync(prod);
            await _produtoRepository.SalvarAsync();

            return prod;
        }

        public async Task<Produto> ListarPorIdAsync(int id)
        {
            var listar = await _produtoRepository.ListarPorIdAsync(id);
            return listar;
        }

        public async Task<ICollection<Produto>> ListarTodosAsync()
        {
            var listar = await _produtoRepository.ListarTodosAsync();
            return listar;
        }

        public async Task RemoverProdutoAsync(int id)
        {
            await _produtoRepository.RemoverProdutoAsync(id);
            await _produtoRepository.SalvarAsync();
        }
    }
}