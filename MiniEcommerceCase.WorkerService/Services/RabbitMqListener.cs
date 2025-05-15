using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using MiniEcommerceCase.WorkerService.Events;
using MiniEcommerceCase.WorkerService.Interfaces;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace MiniEcommerceCase.WorkerService.Services
{
    public class RabbitMqListener
    {
        private readonly IConfiguration _configuration;
        private readonly IRedisLogger _redisLogger;

        public RabbitMqListener(IConfiguration configuration, IRedisLogger redisLogger)
        {
            _configuration = configuration;
            _redisLogger = redisLogger;
        }

        public void StartListening()
        {
            var factory = new ConnectionFactory
            {
                HostName = _configuration["RabbitMq:Host"],
                UserName = _configuration["RabbitMq:Username"],
                Password = _configuration["RabbitMq:Password"]
            };

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(queue: "order-placed",
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var json = Encoding.UTF8.GetString(body);

                var orderEvent = JsonSerializer.Deserialize<OrderPlacedEvent>(json);

                Console.WriteLine($"\n[✓] Order received → OrderId: {orderEvent?.OrderId}");
                Console.WriteLine($"    User: {orderEvent?.UserId}");
                Console.WriteLine($"    Product: {orderEvent?.ProductId}");
                Console.WriteLine($"    Quantity: {orderEvent?.Quantity}");
                Console.WriteLine($"    PaymentMethod: {orderEvent?.PaymentMethod}");

                await Task.Delay(2000);

                await _redisLogger.LogProcessedAsync(orderEvent.OrderId); 


                Console.WriteLine($"[✔] Order processed at {DateTime.UtcNow:HH:mm:ss}");
            };

            channel.BasicConsume(queue: "order-placed", autoAck: true, consumer: consumer);

            Console.WriteLine("🐇 Listening for order-placed messages...");
            Console.ReadLine();
        }
    }
}
