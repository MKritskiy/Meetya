using Friends.Application.Interfaces;
using Friends.Infrastructure.Data;
using Friends.Infrastructure.Repositories;
using Friends.Infrastructure.Services;

namespace Friends.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfigurationManager config, ILogger logger)
    {
        string? connectionString = config.GetConnectionString("ConnectionString");
        services.AddDbContext<FriendDbContext>(options => options.UseNpgsql(connectionString));

        services.AddScoped<IFriendRepository, FriendRepository>();
        services.AddScoped<IFriendService, FriendService>();

        logger.LogInformation("{Project} services registered", "Infrastructure");
        return services;
    }
}
