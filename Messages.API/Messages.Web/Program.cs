using Messages.Application.Interfaces;
using Messages.Infrastructure;
using Messages.Infrastructure.Services;
using Messages.Web.Configurations;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Extensions.Logging;

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
builder.Services.AddHttpClient<IParticipantService, ParticipantService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["EvemtsApiUrl"]);
});
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

app.Run();
