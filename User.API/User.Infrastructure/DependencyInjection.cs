using StackExchange.Redis;
using Users.Application.Interfaces;
using Users.Infrastructure.Data;
using Users.Infrastructure.General;
using Users.Infrastructure.Repositories;
using Users.Infrastructure.Services;

namespace Users.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration config, ILogger logger)
        {
            string? connectionString = config.GetConnectionString("ConnectionString");
            services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(connectionString));
            
            var redis = ConnectionMultiplexer.Connect(config.GetConnectionString("Redis") ?? "");
            services.AddSingleton<IConnectionMultiplexer>(redis);

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IProfileRepository, ProfileRepository>();

            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IProfileService, ProfileService>();

            services.AddSingleton<ICodeGenerator, CodeGenerator>();
            services.AddSingleton<ICodeCacheService, RedisCodeCacheService>();
            services.AddSingleton<IEncrypt, Encrypt>();
            services.AddSingleton<ITokenService, TokenService>();
            services.AddSingleton<INotificationQueueService, RabbitMqNotificationService>(provider =>
            {
                var config = provider.GetService<IConfiguration>();
                var logger = provider.GetService<ILogger<RabbitMqNotificationService>>();
                return RabbitMqNotificationService.CreateAsync(config, logger).GetAwaiter().GetResult();
            });

            logger.LogInformation("{Project} services registered", "Infrastructure");
            return services;
        }
    }
}
