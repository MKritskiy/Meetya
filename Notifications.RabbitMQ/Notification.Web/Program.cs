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
// RabbitMQ
builder.Services.AddSingleton<IConnection>(sp =>
{
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
        .WaitAndRetry(5, retryAttempt =>
            TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
    return policy.Execute(async () => await factory.CreateConnectionAsync()).GetAwaiter().GetResult();
});

builder.Services.Configure<SmtpConfig>(builder.Configuration.GetSection("SmtpConfig"));
builder.Services.AddTransient<IEmailService, SmtpEmailService>();
//builder.Services.AddSingleton<IEmailService, SendGridEmailService>();
builder.Services.AddSingleton<ISmsService, TwilioSmsService>();
builder.Services.AddHostedService<NotificationWorker>();
var connStr = $"amqp://{builder.Configuration["RabbitMQ:Username"]}:{builder.Configuration["RabbitMQ:Password"]}@{builder.Configuration["RabbitMQ:Host"]}:{builder.Configuration["RabbitMQ:Port"]}";
var factory = new ConnectionFactory()
{
    Uri = new Uri(connStr),
    AutomaticRecoveryEnabled = true
};
try
{
    var connection = await factory.CreateConnectionAsync();
    builder.Services
         .AddSingleton(connection)
         .AddHealthChecks()
         .AddCheck("self", () => HealthCheckResult.Healthy("Application is running"))
         .AddRabbitMQ(name: "rabbitmq-check", tags: new[] { "rabbitmq" });
}
catch (Exception e)
{
    Log.Logger.Error("Exception raised: " + e.Message);
}




builder.Services.AddLogging(logging =>
    logging.AddConsole().AddDebug());

var app = builder.Build();

app.UseHealthChecks("/health");
app.MapHealthChecks("/health");

app.Run();
