using Domain.Constants;
using Messages.Application;
using Messages.Application.Event;
using Messages.Application.Interfaces;
using Messages.Application.Users;
using Messages.Infrastructure.Data;
using Messages.Infrastructure.Repositories;
using Messages.Infrastructure.Services;

namespace Messages.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfigurationManager config, ILogger logger)
    {
        string? connectionString = config.GetConnectionString("ConnectionString");
        services.AddDbContext<MessageDbContext>(options => options.UseNpgsql(connectionString));
        services.AddHttpClient<IEventsClientApi, EventsClientApi>((provider, client) =>
        {
            client.BaseAddress = new Uri(GatewayConstants.GATEWAY_INTERNAL_HOST + GatewayConstants.EVENT_API_ROUTE);
        });
        services.AddHttpClient<IUsersApiClient, UsersApiClient>((provider, client) =>
        {
            client.BaseAddress = new Uri(GatewayConstants.GATEWAY_INTERNAL_HOST + GatewayConstants.USER_API_ROUTE);
        });
        services.AddScoped<IMessageService, MessageService>();
        services.AddScoped<IMessageRepository, MessageRepository>();

        logger.LogInformation("{Project} services registered", "Infrastructure");
        return services;
    }
}
