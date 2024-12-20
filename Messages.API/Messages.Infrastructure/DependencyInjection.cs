using Messages.Application.Interfaces;
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

        services.AddScoped<IMessageService, MessageService>();
        services.AddScoped<IMessageRepository, MessageRepository>();
        services.AddScoped<IParticipantService, ParticipantService>();

        logger.LogInformation("{Project} services registered", "Infrastructure");
        return services;
    }
}
