using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using RabbitMQ.Client;
using Shared.Events;
using Shared.Interfaces;

namespace Venda.Infrastructure.Messaging
{
    public class PedidoPublisher : IPedidoPublisher
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public PedidoPublisher()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.ExchangeDeclare(
                    exchange: "pedido_criado_exchange",
                    type: ExchangeType.Fanout,
                    durable: true
            );
        }

        public void PublicarPedidoCriado(PedidoCriadoEvent evento)
        {
            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(evento));

            _channel.BasicPublish(
                exchange: "pedido_criado_exchange",
                routingKey: "",
                basicProperties: null,
                body: body
            );

            Console.WriteLine($"[x] Evento PedidoCriado publicado: {evento.PedidoId}");
        }
    }
}