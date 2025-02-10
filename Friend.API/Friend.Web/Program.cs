using Domain.Constants;
using Friends.Infrastructure;
using Friends.Web.Configurations;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Extensions.Logging;
using Web.Configurations;

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
        Title = "Teledock API",
        Version = "v1"
    });
});

#if (aspire)
builder.AddServiceDefaults();
#endif
var app = builder.Build();
await app.UseAppMiddlewareAndSeedDatabase();
await app.UseAppSwaggerOpenApiServers(
    new List<OpenApiServer>
    {
        new OpenApiServer { Url = GatewayConstants.GATEWAY_EXTERNAL_HOST + GatewayConstants.FRIEND_API_ROUTE },
        new OpenApiServer { Url = GatewayConstants.FRIEND_CONTAINER_EXTERNAL_HOST}
    });
app.Run();