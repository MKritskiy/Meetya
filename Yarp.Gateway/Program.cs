var builder = WebApplication.CreateBuilder(args);

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

builder.Services.AddCors(options =>
{
    options.AddPolicy("customPolicy", builder =>
    {
        builder.WithOrigins("http://127.0.0.1:5500", "https://when-and-where.ru", "https://www.when-and-where.ru")
               .AllowAnyMethod()
               .AllowAnyHeader()
               .AllowCredentials();
    });
});

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(8080); // HTTP
    options.ListenAnyIP(443, listenOptions =>
    {
        listenOptions.UseHttps("/app/certs/cert.pfx");
    });
});
builder.Services.AddHealthChecks();
var app = builder.Build();

app.UseHttpsRedirection(); // Перенаправление HTTP на HTTPS
app.UseHsts(); // Включение HSTS
app.UseWebSockets();
app.UseCors("customPolicy");

app.UseHealthChecks("/health");
app.MapHealthChecks("/health");

app.MapReverseProxy(proxyPipeline =>
{
    proxyPipeline.UseWebSockets();
    proxyPipeline.UseSessionAffinity();
    proxyPipeline.UseLoadBalancing();
});

app.Run();