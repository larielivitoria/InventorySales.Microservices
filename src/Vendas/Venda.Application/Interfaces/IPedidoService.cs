using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Venda.Application.DTOs;
using Venda.Domain.Entities;

namespace Venda.Application.Interfaces
{
    public interface IPedidoService
    {
        public Task<Pedido> CriarPedidoAsync(PedidoDTO pedidoDTO);
        public Task<Pedido> AtualizarPedidoAsync(int id, AtualizarStatusDTO atualizarStatusDTO);
        public Task RemoverPedidoAsync(int id);
        public Task<ICollection<Pedido>> ListarTodosAsync();
        public Task<Pedido> ListarPorIdAsync(int id);
    }
}