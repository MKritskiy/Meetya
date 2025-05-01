using Microsoft.Extensions.Diagnostics.HealthChecks;
using Notification.Web.Interfaces;
using Notification.Web.Models;
using Notification.Web.Services;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using Serilog;
using Serilog.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);
var logger = Log.Logger = new LoggerConfiguration()
  .Enrich.FromLogContext()
  .WriteTo.Console()
  .CreateLogger();
builder.Host.UseSerilog((_, config) => config.ReadFrom.Configuration(builder.Configuration));
logger.Information("Starting web host");

var appLogger = new SerilogLoggerFactory(logger)
    .CreateLogger<Program>();

var factory = new ConnectionFactory
{
    HostName = builder.Configuration["RabbitMQ:Host"],
    Port = int.Parse(builder.Configuration["RabbitMQ:Port"]),
    UserName = builder.Configuration["RabbitMQ:Username"],
    Password = builder.Configuration["RabbitMQ:Password"],
    ConsumerDispatchConcurrency = 4,
    AutomaticRecoveryEnabled = true
};
var policy = Policy
    .Handle<BrokerUnreachableException>()
    .WaitAndRetryForeverAsync(retryAttempt =>
        TimeSpan.FromSeconds(Math.Pow(2, Math.Min(retryAttempt, 6))),
        (exception, retryCount, waitTime) =>
        {
            Log.Logger.Information($"Retrying RabbitMQ connection (attempt {retryCount}) after {waitTime.TotalSeconds} seconds... Error: {exception.Message}");
        });
builder.Services.AddHealthChecks()
    .AddCheck("self", () => HealthCheckResult.Healthy("Application is running"));
IConnection connection = null;
try
{
    connection = await policy.ExecuteAsync(async () =>
    {
        Log.Logger.Information("Attempting to connect to RabbitMQ...");
        return await factory.CreateConnectionAsync();
    });
    Log.Logger.Information("Successfully connected to RabbitMQ.");
    builder.Services.AddSingleton(connection);
    // Add RabbitMQ health check only if connection succeeds
    builder.Services.AddHealthChecks()
        .AddRabbitMQ(name: "rabbitmq-check", tags: new[] { "rabbitmq" });
}
catch (Exception e)
{
    Log.Logger.Error($"Failed to connect to RabbitMQ: {e.Message}. Health checks for RabbitMQ will not be added.");
}


if (connection == null)
{
    Log.Logger.Warning("RabbitMQ connection not established. Health checks for RabbitMQ will not be added.");
}
builder.Services.Configure<SmtpConfig>(builder.Configuration.GetSection("SmtpConfig"));
builder.Services.AddTransient<IEmailService, SmtpEmailService>();
//builder.Services.AddSingleton<IEmailService, SendGridEmailService>();
builder.Services.AddSingleton<ISmsService, TwilioSmsService>();
builder.Services.AddHostedService<NotificationWorker>();



builder.Services.AddLogging(logging =>
    logging.AddConsole().AddDebug());

var app = builder.Build();

app.MapHealthChecks("/health");

app.Run();
