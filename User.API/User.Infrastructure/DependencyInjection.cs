using Users.Application.Interfaces;
using Users.Infrastructure.Data;
using Users.Infrastructure.General;
using Users.Infrastructure.Repositories;
using Users.Infrastructure.Services;

namespace Users.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, ConfigurationManager config, ILogger logger)
        {
            string? connectionString = config.GetConnectionString("ConnectionString");
            services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(connectionString));

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IProfileRepository, ProfileRepository>();

            services.AddScoped<IEncrypt, Encrypt>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IProfileService, ProfileService>();


            logger.LogInformation("{Project} services registered", "Infrastructure");
            return services;
        }
    }
}
