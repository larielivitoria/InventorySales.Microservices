using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Venda.Domain.Entities;

namespace Venda.Domain.Interfaces
{
    public interface IPedidoRepository
    {
        public Task<Pedido> CriarPedidoAsync(Pedido pedido);
        public Pedido AtualizarPedido(Pedido pedido);
        public Task<ICollection<Pedido>> ListarTodosAsync();
        public Task<Pedido> ListarPorIdAsync(int id);
        public Task SalvarAsync();

    }
}