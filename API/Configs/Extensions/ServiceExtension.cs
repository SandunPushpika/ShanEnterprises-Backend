using Application.Interfaces.Repositories;
using Application.Repositories;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace ShanEnterprises.Configs.Extensions;

public static class ServiceExtension
{
    public static IServiceCollection AddRequiredServices(this IServiceCollection services)
    {
        services.AddRepositories();
        return services;
    }

    public static void AddDatabaseConfig(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseNpgsql(connectionString);
        });
    }

    private static void AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
    }
}