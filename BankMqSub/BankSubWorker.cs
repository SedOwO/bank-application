using BankMqSub.Configs;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Data.Common;
using System.Text;
using System.Threading.Channels;

namespace BankMqSub
{
    public class BankSubWorker : BackgroundService
    {
        private readonly RabbitMqSettings _settings;
        private readonly ILogger<BankSubWorker> _logger;


        public BankSubWorker(IOptions<RabbitMqSettings> options, ILogger<BankSubWorker> logger)
        {
            _settings = options.Value;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var factory = new ConnectionFactory
            {
                HostName = _settings.HostName,
                UserName = _settings.UserName,
                Password = _settings.Password,
                VirtualHost = _settings.VirtualHost
            };

            using var connection = await factory.CreateConnectionAsync();
            using var channel = await connection.CreateChannelAsync();

            await channel.ExchangeDeclareAsync(
                exchange: _settings.ExchangeName,
                type: ExchangeType.Direct,
                durable: true,
                autoDelete: false
            );

            await channel.QueueDeclareAsync(
                queue: _settings.QueueName,
                durable: true,
                exclusive: false,
                autoDelete: false
            );

            await channel.QueueBindAsync(
                queue: _settings.QueueName,
                exchange: _settings.ExchangeName,
                routingKey: _settings.RoutingKey
            );

            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.ReceivedAsync += async (model, ea) =>
            {
                try
                {
                    var message = Encoding.UTF8.GetString(ea.Body.ToArray());
                    //Console.WriteLine($"[x] Received: {message}");
                    _logger.LogInformation(message);

                    await channel.BasicAckAsync(ea.DeliveryTag, multiple: false);
                }
                catch (Exception e)
                {
                    //Console.WriteLine($"[!] Error Processing message. {e.Message}");
                    _logger.LogInformation($"Error: {e.Message}");
                    await channel.BasicNackAsync(ea.DeliveryTag, false, true);
                }
            };

            await channel.BasicConsumeAsync(
                queue: _settings.QueueName,
                autoAck: false,
                consumer: consumer
            );

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }



    }
}
