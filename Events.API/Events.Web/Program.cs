using Microsoft.OpenApi.Models;
using Serilog.Extensions.Logging;
using Serilog;
using Events.Infrastructure;
using Events.Web.Configurations;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);


var logger = Log.Logger = new LoggerConfiguration()
  .Enrich.FromLogContext()
  .WriteTo.Console()
  .CreateLogger();

logger.Information("Starting web host");

builder.AddLoggerConfigs();
builder.Services.AddControllers();
var appLogger = new SerilogLoggerFactory(logger)
    .CreateLogger<Program>();

builder.Services.AddInfrastructureServices(builder.Configuration, appLogger);
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Meetya API",
        Version = "v1"
    });
});

#if (aspire)
builder.AddServiceDefaults();
#endif
var app = builder.Build();
await app.UseAppMiddlewareAndSeedDatabase();

app.Run();