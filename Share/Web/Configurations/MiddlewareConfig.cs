
using Microsoft.AspNetCore.Builder;
using Microsoft.OpenApi.Models;

namespace Web.Configurations;

public static class MiddlewareConfig
{
    public static async Task<IApplicationBuilder> UseAppSwaggerOpenApiServers(this WebApplication app, List<OpenApiServer> servers)
    {
        app.UseSwagger(c =>
        {
            c.PreSerializeFilters.Add((swaggerDoc, httpReq) =>
            {
                swaggerDoc.Servers = servers;
            });
        });
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("./swagger/v1/swagger.json", "Meetya API v1");
            c.RoutePrefix = string.Empty;
        });
        return app;
    }
}
