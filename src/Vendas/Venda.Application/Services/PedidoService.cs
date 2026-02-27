using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Shared.Events;
using Shared.Interfaces;
using Shared.Messaging.RPC;
using Venda.Application.DTOs;
using Venda.Application.Interfaces;
using Venda.Domain.Entities;
using Venda.Domain.Interfaces;

namespace Venda.Application.Services
{
    public class PedidoService : IPedidoService
    {
        private readonly IPedidoRepository _pedidoRepository;
        private readonly IRabbitMQRequestBus _bus;
        private readonly IPedidoPublisher _pedidoPublisher;

        public PedidoService(IPedidoRepository pedidoRepository, IRabbitMQRequestBus bus, IPedidoPublisher pedidoPublisher)
        {
            _pedidoRepository = pedidoRepository;
            _bus = bus;
            _pedidoPublisher = pedidoPublisher;
        }

        public async Task<Pedido> AtualizarPedidoAsync(int id, AtualizarStatusDTO atualizarStatusDTO)
        {
            var pedido = await _pedidoRepository.ListarPorIdAsync(id);

            pedido.Status = atualizarStatusDTO.Status;

            _pedidoRepository.AtualizarPedido(pedido);
            await _pedidoRepository.SalvarAsync();

            return pedido;
        }

        public async Task<Pedido> CriarPedidoAsync(PedidoDTO pedidoDTO)
        {
            //verifica estoque de todos os itens via RPC
            foreach (var item in pedidoDTO.Itens)
            {
                var request = new VerificarEstoqueRequest
                {
                    ProdutoId = item.ProdutoId,
                    Quantidade = item.QuantidadeItemPedido
                };

                var response = await _bus.RequestAsync<VerificarEstoqueRequest, VerificarEstoqueResponse>(request);

                if (!response.TemEstoque)
                {
                    //lança exceção se algum item não tiver estoque
                    throw new InvalidOperationException($"Não há estoque suficiente para o produto {item.ProdutoId}.");
                }
            }

            //validações
            if (string.IsNullOrWhiteSpace(pedidoDTO.ClienteNome))
            {
                throw new Exception("Nome do cliente não pode estar vazio.");
            }

            if (string.IsNullOrWhiteSpace(pedidoDTO.Status))
            {
                throw new Exception("Status do pedido não pode estar vazio.");
            }

            if (pedidoDTO.Itens.Any(i => i.ProdutoId <= 0))
            {
                throw new Exception("Todos os itens do pedido devem ter o ProdutoId preenchido.");
            }

            if (pedidoDTO.Itens.Any(i => i.QuantidadeItemPedido <= 0))
            {
                throw new Exception("Todos os itens do pedido devem ter o QuantidadeItemPedido preenchido.");
            }

            //criação do pedido
            var pedido = new Pedido
            {
                ClienteNome = pedidoDTO.ClienteNome,
                Status = pedidoDTO.Status,
                Itens = pedidoDTO.Itens.Select(i => new ItemPedido
                {
                    ProdutoId = i.ProdutoId,
                    QuantidadeItemPedido = i.QuantidadeItemPedido
                }).ToList()
            };

            await _pedidoRepository.CriarPedidoAsync(pedido);
            await _pedidoRepository.SalvarAsync();

            //publicar evento
            var evento = new PedidoCriadoEvent
            {
                PedidoId = pedido.PedidoId,
                ItensEvent = pedido.Itens?.Select(i => new ItemPedidoEvent
                {
                    ProdutoId = i.ProdutoId,
                    QuantidadeItemEvent = i.QuantidadeItemPedido
                }).ToList() ?? new List<ItemPedidoEvent>()
            };

            await Task.Run(() => _pedidoPublisher.PublicarPedidoCriado(evento));

            return pedido;
        }

        public async Task<Pedido> ListarPorIdAsync(int id)
        {
            var listar = await _pedidoRepository.ListarPorIdAsync(id);
            return listar;
        }

        public async Task<ICollection<Pedido>> ListarTodosAsync()
        {
            var listar = await _pedidoRepository.ListarTodosAsync();
            return listar;
        }

        public async Task RemoverPedidoAsync(int id)
        {
            var pedido = await _pedidoRepository.ListarPorIdAsync(id);

            pedido.Status = "Cancelado";

            _pedidoRepository.AtualizarPedido(pedido);
            await _pedidoRepository.SalvarAsync();
        }
    }
}