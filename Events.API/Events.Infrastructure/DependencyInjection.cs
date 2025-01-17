using Domain.Constants;
using Events.Application;
using Events.Application.Interfaces;
using Events.Infrastructure.Data;
using Events.Infrastructure.Repositories;
using Events.Infrastructure.Services;

namespace Events.Infrastructure;


public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfigurationManager config, ILogger logger)
    {
        string? connectionString = config.GetConnectionString("ConnectionString");
        services.AddDbContext<EventDbContext>(options => options.UseNpgsql(connectionString));

        services.AddHttpClient<IUsersApiClient, UsersApiClient>((provider, client) =>
        {
            client.BaseAddress = new Uri(GatewayConstants.GATEWAY_INTERNAL_HOST + GatewayConstants.USER_API_ROUTE);
        });

        services.AddScoped<IPollRepository, PollRepository>();
        services.AddScoped<IEventParticipantRepository, EventParticipantRepository>();
        services.AddScoped<IEventRepository,EventRepository>();

        services.AddScoped<IEventService, EventService>();
        services.AddScoped<IPollService, PollService>();
        services.AddScoped<IParticipantService, ParticipantService>();

        logger.LogInformation("{Project} services registered", "Infrastructure");
        return services;
    }
}
