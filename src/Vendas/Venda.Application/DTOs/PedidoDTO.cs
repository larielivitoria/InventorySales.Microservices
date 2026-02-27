using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Venda.Application.DTOs
{
    public class PedidoDTO
    {
        public string ClienteNome { get; set; }
        public string Status { get; set; }
        public ICollection<ItemPedidoDTO> Itens { get; set; } = new List<ItemPedidoDTO>();
    }
}