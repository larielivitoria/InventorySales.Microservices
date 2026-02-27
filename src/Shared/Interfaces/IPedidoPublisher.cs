using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Shared.Events;

namespace Shared.Interfaces
{
    public interface IPedidoPublisher
    {
        void PublicarPedidoCriado(PedidoCriadoEvent evento);
    }
}