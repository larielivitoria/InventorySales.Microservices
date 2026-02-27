using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Venda.Domain.Entities;
using Venda.Domain.Interfaces;
using Venda.Infrastructure.Db;

namespace Venda.Infrastructure.Repositorys
{
    public class PedidoRepository : IPedidoRepository
    {
        private readonly VendaContext _vendaContext;
        public PedidoRepository(VendaContext vendaContext)
        {
            _vendaContext = vendaContext;
        }

        public Pedido AtualizarPedido(Pedido pedido)
        {
            _vendaContext.Pedidos.Update(pedido);
            return pedido;
        }

        public async Task<Pedido> CriarPedidoAsync(Pedido pedido)
        {
            await _vendaContext.Pedidos.AddAsync(pedido);
            return pedido;
        }

        public async Task<Pedido> ListarPorIdAsync(int id)
        {
            var listar = await _vendaContext.Pedidos.FindAsync(id);
            if (listar == null)
            {
                throw new Exception($"Pedido {id} não encontrado.");
            }
            return listar;
        }

        public async Task<ICollection<Pedido>> ListarTodosAsync()
        {
            var todos = await _vendaContext.Pedidos.ToListAsync();
            return todos;
        }
        
        public async Task SalvarAsync()
        {
            await _vendaContext.SaveChangesAsync();
        }
    }
}