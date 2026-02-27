using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shared.Messaging.RPC
{
    public interface IRabbitMQRequestBus
    {
        Task<TResponse> RequestAsync<TRequest, TResponse>(TRequest request);

        void RespondAsync<TRequest, TResponse>(Func<TRequest, Task<TResponse>> handler);
    }
}