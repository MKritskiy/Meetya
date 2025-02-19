using Domain.Constants;
using Files.Web.Configurations;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Extensions.Logging;
using Web.Configurations;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

var logger = Log.Logger = new LoggerConfiguration()
  .Enrich.FromLogContext()
  .WriteTo.Console()
  .CreateLogger();

builder.Services.Configure<RouteOptions>(options =>
{
    options.LowercaseUrls = true;
    options.LowercaseQueryStrings = true;
});


builder.Services.Configure<FormOptions>(options => { 
    options.MultipartBodyLengthLimit = 1024 * 1024 * 1024; 
});


builder.AddLoggerConfigs();


builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Meetya API",
        Version = "v1"
    });
});


var appLogger = new SerilogLoggerFactory(logger)
    .CreateLogger<Program>();



#if (aspire)
builder.AddServiceDefaults();
#endif
var app = builder.Build();



app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(builder.Environment.ContentRootPath, "wwwroot")),
    RequestPath = "/static",
    OnPrepareResponse = ctx =>
    {
        ctx.Context.Response.Headers.Append("Cache-Control", "public,max-age=604800");
    }
});


if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseHsts();
}


app.MapControllers();
await app.UseAppSwaggerOpenApiServers(
    new List<OpenApiServer>
    {
        new OpenApiServer { Url = GatewayConstants.GATEWAY_EXTERNAL_HOST + GatewayConstants.FILE_API_ROUTE },
        new OpenApiServer { Url = GatewayConstants.FILE_CONTAINER_EXTERNAL_HOST}
    });
app.Run();
