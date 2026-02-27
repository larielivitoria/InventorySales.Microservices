using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Estoque.Domain.Interfaces;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared.Events;

namespace Estoque.Infrastructure.Messaging
{
    public class PedidoCriadoConsumer
    {
        private readonly RabbitMQ.Client.IModel _channel;
        private readonly IServiceScopeFactory _scopeFactory;

        public PedidoCriadoConsumer(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;

            var factory = new RabbitMQ.Client.ConnectionFactory() { HostName = "localhost" };
            var connection = factory.CreateConnection();
            _channel = connection.CreateModel();

            //Exchange do tipo fanout, mesmo nome usado no publisher
            _channel.ExchangeDeclare(exchange: "pedido_criado_exchange", type: ExchangeType.Fanout, durable: true);

            //Criar fila temporária
            var queueName = "pedido_criado_fila";
            _channel.QueueDeclare(
                queue: queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            _channel.QueueBind(queue: queueName, exchange: "pedido_criado_exchange", routingKey: "");

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (ch, ea) =>
            {
                Task.Run(async () => await ProcessarEventoAsync(ea));
            };

            _channel.BasicConsume(queue: queueName, autoAck: false, consumer: consumer);
            Console.WriteLine("Consumer iniciado pronto para consumir mensagens.");
        }

        private async Task ProcessarEventoAsync(BasicDeliverEventArgs ea)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var produtoRepository = scope.ServiceProvider.GetRequiredService<IProdutoRepository>();

                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var evento = JsonSerializer.Deserialize<PedidoCriadoEvent>(message);
                if (evento == null || evento.ItensEvent == null || !evento.ItensEvent.Any())
                {
                    Console.WriteLine("Evento inválido ou sem itens. Ignorando.");
                    return;
                }

                foreach (var item in evento.ItensEvent)
                {
                    //Baixa do estoque
                    await produtoRepository.DiminuirEstoqueAsync(item.ProdutoId, item.QuantidadeItemEvent);
                    Console.WriteLine($"Diminuindo estoque do Produto {item.ProdutoId} em {item.QuantidadeItemEvent}");
                }

                await produtoRepository.SalvarAsync();

                //Confirmar que a mensagem foi processada
                _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                Console.WriteLine("Baixa no estoque concluída.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao processar evento: {ex.Message}");
            }
        }
    }
}