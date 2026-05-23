using System.Reflection;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Application.Repositories;
using Application.Services;
using Core.Enums;
using Core.Validators;
using FluentValidation;
using FluentValidation.AspNetCore;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Application.Mappers;
namespace ShanEnterprises.Configs.Extensions;

public static class ServiceExtension
{
    public static IServiceCollection AddRequiredServices(this IServiceCollection services)
    {
        services.AddRepositories();
        services.AddServices();
        services.RegisterValidators();
        services.AddAutoMapper(cfg => { }, typeof(UserProfile).Assembly); 
        return services;
    }

    private static void AddServices(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IVehicleService, VehicleService>();
        //Register all services here
    }
    
    private static void AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IVehicleRepository, VehicleRepository>();
        //Register all repositories here
    }

    private static void RegisterValidators(this IServiceCollection services)
    {
        services.AddFluentValidationAutoValidation();
        services.AddValidatorsFromAssemblyContaining<ValidationAssemblyHelper>();
    }
    
    public static void AddDatabaseConfig(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseNpgsql(connectionString,
                o =>
                {
                    o.MapEnum<UserRole>("user_role");
                    o.MapEnum<UserStatus>("user_status");
                    o.MapEnum<VehicleStatus>("vehicle_status");
                    o.MapEnum<FuelType>("fuel_type");
                    o.MapEnum<TransmissionType>("transmission_type");
                });
            options.UseSnakeCaseNamingConvention();
        });
    }
}