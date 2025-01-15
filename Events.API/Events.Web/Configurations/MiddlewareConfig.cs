﻿using Domain.Constants;
using Events.Infrastructure.Data;
using Microsoft.OpenApi.Models;

namespace Events.Web.Configurations;

public static class MiddlewareConfig
{
    public static async Task<IApplicationBuilder> UseAppMiddlewareAndSeedDatabase(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseHsts();
        }

        app.UseSwagger(c =>
        {
            c.PreSerializeFilters.Add((swaggerDoc, httpReq) =>
            {
                swaggerDoc.Servers = new List<OpenApiServer>
                    {
                        new OpenApiServer { Url = GatewayConstants.GATEWAY_EXTERNAL_HOST + GatewayConstants.EVENT_API_ROUTE },
                        new OpenApiServer { Url = GatewayConstants.EVENT_CONTAINER_EXTERNAL_HOST}
                    };
            });
        });
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("./swagger/v1/swagger.json", "Meetya API v1");
            c.RoutePrefix = string.Empty;
        });

        //app.UseHttpsRedirection(); // Note this will drop Authorization headers

        await SeedDatabase(app);
        app.MapControllers();
        return app;
    }

    static async Task SeedDatabase(WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;

        try
        {
            var context = services.GetRequiredService<EventDbContext>();
            //          context.Database.Migrate();
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
        }
        catch (Exception ex)
        {
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "An error occurred seeding the DB. {exceptionMessage}", ex.Message);
        }
    }
}
