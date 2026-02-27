using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Venda.Domain.Entities
{
    public class Pedido
    {
        [Key]
        public int PedidoId { get; set; }
        public string ClienteNome { get; set; }
        [Required]
        public string Status { get; set; }
        public ICollection<ItemPedido> Itens { get; set; } = new List<ItemPedido>();
    }
}