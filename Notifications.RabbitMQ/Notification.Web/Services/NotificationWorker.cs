using Domain.Constants;
using Notification.Web.Interfaces;
using Notification.Web.Models;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text.Json;
namespace Notification.Web.Services;

public class NotificationWorker : BackgroundService
{
    private IChannel _channel;
    private readonly IEmailService _emailService;
    private readonly ISmsService _smsService;
    private readonly ILogger<NotificationWorker> _logger;
    private readonly IConnection _connection;

    public NotificationWorker(
    IConnection connection,
    IEmailService emailService,
    ISmsService smsService,
    ILogger<NotificationWorker> logger)
    {
        _connection = connection;
        _emailService = emailService;
        _smsService = smsService;
        _logger = logger;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    { 
        _channel = await _connection.CreateChannelAsync();
        await _channel.QueueDeclareAsync(
            queue: HelpersConstants.QUEUE_NAME,
            durable: true,
            exclusive: false,
            autoDelete: false);

        var consumer = new AsyncEventingBasicConsumer(_channel);

        consumer.ReceivedAsync += async (_, ea) =>
        {
            try
            {
                Serilog.Log.Logger.Information($"Processing...");
                await ProcessMessage(ea);
                await _channel.BasicRejectAsync(ea.DeliveryTag, false);
            }
            catch (Exception ex)
            {
                Serilog.Log.Logger.Error(ex, "Error processing message");
                await _channel.BasicNackAsync(ea.DeliveryTag, false, true);
            }
        };

        await _channel.BasicConsumeAsync("notifications", false, consumer);

        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);
        }
    }

    private async Task ProcessMessage(BasicDeliverEventArgs ea)
    {
        var body = ea.Body.ToArray();
        var message = JsonSerializer.Deserialize<NotificationMessage>(body);
        Serilog.Log.Logger.Information($"Processing {message.Type} notification for {message.Target}");

        switch (message.Type.ToLower())
        {
            case "email":
                var policy = Policy
                    .Handle<Exception>()
                    .WaitAndRetryAsync(3, attempt => TimeSpan.FromSeconds(attempt * 2));

                await policy.ExecuteAsync(async () =>
                    await _emailService.SendEmailAsync(message.Target, message.Code));
                break;

            case "sms":
                await _smsService.SendSmsAsync(message.Target, message.Code);
                break;

            default:
                _logger.LogWarning($"Unknown notification type: {message.Type}");
                break;
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await base.StopAsync(cancellationToken);
        await _channel?.CloseAsync();
        await _connection?.CloseAsync();
    }
}
