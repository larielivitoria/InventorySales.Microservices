using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Venda.Domain.Entities
{
    public class ItemPedido
    {
        [Key]
        public int ItemId { get; set; }
        public int ProdutoId { get; set; }
        public int QuantidadeItemPedido { get; set; }
    }
}