using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared.Messaging;
using Shared.Messaging.RPC;

namespace Estoque.Infrastructure.Messaging
{
    public class RabbitMQRequestBus : IRabbitMQRequestBus
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly ConcurrentDictionary<string, TaskCompletionSource<string>> _pendingResponses = new();

        public RabbitMQRequestBus()
        {
            var factory = new ConnectionFactory
            {
                HostName = "localhost",
                DispatchConsumersAsync = true
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
        }

        public Task<TResponse> RequestAsync<TRequest, TResponse>(TRequest request)
        {
            var tcs = new TaskCompletionSource<string>();
            var correlationId = Guid.NewGuid().ToString();

            //fila temporária para receber resposta
            var replyQueue = _channel.QueueDeclare().QueueName;

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.Received += async (sender, ea) =>
            {
                if (ea.BasicProperties.CorrelationId == correlationId)
                {
                    var body = Encoding.UTF8.GetString(ea.Body.ToArray());
                    tcs.SetResult(body);
                }
                await Task.Yield();
            };

            _channel.BasicConsume(replyQueue, true, consumer);

            var props = _channel.CreateBasicProperties();
            props.CorrelationId = correlationId;
            props.ReplyTo = replyQueue;

            var messageBody = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(request));

            //envia para a fila do Estoque
            _channel.BasicPublish(
                exchange: "",
                routingKey: typeof(TRequest).Name + "_request_queue",
                basicProperties: props,
                body: messageBody
            );

            //transforma a resposta em objeto TResponse
            return tcs.Task.ContinueWith(task =>
            {
                var result = JsonSerializer.Deserialize<TResponse>(task.Result);
                if (result == null)
                {
                    throw new InvalidOperationException("Falha ao desserializar a resposta.");
                }
                
                return result;
            });
        }

        public void RespondAsync<TRequest, TResponse>(Func<TRequest, Task<TResponse>> handler)
        {
            var queueName = typeof(TRequest).Name + "_request_queue";

            _channel.QueueDeclare(
                queue: queueName,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.Received += async (sender, ea) =>
            {
                var body = Encoding.UTF8.GetString(ea.Body.ToArray());
                var requestObj = JsonSerializer.Deserialize<TRequest>(body);
                if (requestObj == null)
                {
                    throw new InvalidOperationException("Requisição recebida é nula");
                }

                //executa o handler da Application
                var responseObj = await handler(requestObj);

                var responseBytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(responseObj));

                var props = _channel.CreateBasicProperties();
                props.CorrelationId = ea.BasicProperties.CorrelationId;

                //envia a resposta de volta
                _channel.BasicPublish(
                    exchange: "",
                    routingKey: ea.BasicProperties.ReplyTo,
                    basicProperties: props,
                    body: responseBytes
                );

                await Task.Yield();
            };

            _channel.BasicConsume(queueName, true, consumer);
        }
    }
}