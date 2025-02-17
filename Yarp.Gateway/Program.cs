var builder = WebApplication.CreateBuilder(args);


builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

builder.Services.AddCors(options =>
{
    options.AddPolicy("customPolicy", builder =>
    {
        builder.WithOrigins("http://127.0.0.1:5500")
               .AllowAnyMethod()
               .AllowAnyHeader()
               .AllowCredentials();
    });
});

var app = builder.Build();

app.UseWebSockets();
app.UseCors("customPolicy");
app.MapReverseProxy(proxyPipeline =>
{
    proxyPipeline.UseWebSockets();
    proxyPipeline.UseSessionAffinity();
    proxyPipeline.UseLoadBalancing();
});



app.Run();
