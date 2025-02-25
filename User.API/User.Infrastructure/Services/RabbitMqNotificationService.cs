using Domain.Constants;
using Microsoft.EntityFrameworkCore.Metadata;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using System.Text.Json;
using Users.Application.Interfaces;
using Users.Domain.QueueEntities;

namespace Users.Infrastructure.Services;

public class RabbitMqNotificationService : INotificationQueueService
{
    private IConnection _connection;
    private IChannel _channel;
    private readonly ILogger<RabbitMqNotificationService> _logger;
    private bool _disposed;

    private RabbitMqNotificationService(
        IConnection connection,
        IChannel channel,
        ILogger<RabbitMqNotificationService> logger)
    {
        _connection = connection;
        _channel = channel;
        _logger = logger;
    }

    public static async Task<RabbitMqNotificationService> CreateAsync(
        IConfiguration config,
        ILogger<RabbitMqNotificationService> logger)
    {
        var factory = new ConnectionFactory()
        {
            HostName = config["RabbitMQ:Host"],
            Port = int.Parse(config["RabbitMQ:Port"]),
            UserName = config["RabbitMQ:Username"],
            Password = config["RabbitMQ:Password"],
            ConsumerDispatchConcurrency = 4 // Важно для асинхронной обработки
        };


        IConnection connection = null;
        IChannel channel = null;

        var policy = Policy
            .Handle<BrokerUnreachableException>()
            .WaitAndRetryAsync(3, attempt =>
        TimeSpan.FromSeconds(Math.Pow(2, attempt)));


        // Объявляем очередь с durability
        await policy.ExecuteAsync(async () =>
        {

            connection = await factory.CreateConnectionAsync();
            channel = await connection.CreateChannelAsync();

            await channel.QueueDeclareAsync(
                queue: HelpersConstants.QUEUE_NAME,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);
        });

        return new RabbitMqNotificationService(connection, channel, logger);
    }

    public async Task PublishNotification(NotificationMessage message)
    {
        if (_disposed) throw new ObjectDisposedException("Service already disposed");
        try
        {
            var body = JsonSerializer.SerializeToUtf8Bytes(message);

            var properties = new BasicProperties();
            properties.Persistent = true;
            properties.MessageId = Guid.NewGuid().ToString();
            properties.Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds());


            await _channel.BasicPublishAsync(
                exchange: "",
                routingKey: HelpersConstants.QUEUE_NAME,
                mandatory: true,
                basicProperties: properties,
                body: body);
            _logger.LogDebug("Published message {MessageId}", properties.MessageId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error publicshing notification");
            
            throw;
        }
    }


    public async ValueTask DisposeAsync()
    {
        if (_disposed) return;

        try
        {
            if (_channel?.IsOpen == true)
                await _channel.CloseAsync();

            if (_connection?.IsOpen == true)
                await _connection.CloseAsync();
        }
        finally
        {
            _channel?.Dispose();
            _connection?.Dispose();
            _disposed = true;
        }
    }
}
