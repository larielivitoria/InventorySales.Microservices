using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Estoque.Domain.Interfaces;
using Shared.Messaging.RPC;

namespace Estoque.Application.Handlers
{
    public class VerificarEstoqueHandler
    {
        private readonly IProdutoRepository _produtoRepository;

        public VerificarEstoqueHandler(IProdutoRepository produtoRepository)
        {
            _produtoRepository = produtoRepository;
        }

        public async Task<VerificarEstoqueResponse> Handle(VerificarEstoqueRequest request)
        {
            
            bool temEstoque = await ValidarEstoque(request.ProdutoId, request.Quantidade);

            return new VerificarEstoqueResponse
            {
                TemEstoque = temEstoque
            };
        }

        private async Task<bool> ValidarEstoque(int produtoId, int quantidade)
        {
            var produto = await _produtoRepository.ListarPorIdAsync(produtoId);
            if (produto == null)
            {
                return false;
            }

            return produto.QuantidadeEstoque >= quantidade; 
        }
    }
}