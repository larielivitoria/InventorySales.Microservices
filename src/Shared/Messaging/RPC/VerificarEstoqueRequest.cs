using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shared.Messaging.RPC
{
    public class VerificarEstoqueRequest
    {
        public int ProdutoId { get; set; }
        public int Quantidade { get; set; }
    }
}