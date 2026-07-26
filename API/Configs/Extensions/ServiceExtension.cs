using System.Security.Claims;
using System.Text;
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
using Core.Helpers;
using Infrastructure.Interfaces;
using Infrastructure.Services;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace ShanEnterprises.Configs.Extensions;

public static class ServiceExtension
{
    public static IServiceCollection AddRequiredServices(this IServiceCollection services)
    {
        services.AddRepositories();
        services.AddServices();
        services.RegisterValidators();
        services.AddCorsConfig();
        services.AddHttpClient();
        services.AddAutoMapper(cfg => { }, typeof(UserProfile).Assembly);
        services.AddMemoryCache();
        return services;
    }

    private static void AddServices(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IVehicleService, VehicleService>();
        services.AddScoped<IContextService, ContextService>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IStorageService, BlobService>();
        services.AddScoped<IImageCompressor, ImageCompressor>();
        services.AddKeyedScoped<IOAuthService, GoogleOAuthService>(OAuthProvider.GOOGLE);
        services.AddScoped<IVehicleMaintenanceService, VehicleMaintenanceService>();
        services.AddScoped<IPaymentService, StripeService>();
        services.AddScoped<IDriverService, DriverService>();
        services.AddScoped<IBookingService, BookingService>();
        services.AddScoped<IReviewService, ReviewService>();
        services.AddScoped<IRecommendationService, RecommendationService>();
        //Register all services here
    }
    
    private static void AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IVehicleRepository, VehicleRepository>();
        services.AddScoped<IVerificationCodeRepository, VerificationCodeRepository>();
        services.AddScoped<IVehicleMaintenanceRepository, VehicleMaintenanceRepository>();

        services.AddScoped<IPaymentRepository, PaymentRepository>();
        services.AddScoped<IDriverRepository, DriverRepository>();
        services.AddScoped<IBookingRepository, BookingRepository>();
        services.AddScoped<IReviewRepository, ReviewRepository>();
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
                    o.MapEnum<BookingStatus>("booking_status");
                    o.MapEnum<MaintenanceStatus>("maintenance_status");
                    o.MapEnum<PaymentStatus>("payment_status");
                    o.MapEnum<PaymentMethod>("payment_method");
                    o.MapEnum<DriverStatus>("driver_status");
                    o.MapEnum<AvailabilityStatus>("availability_status");
                });
            options.UseSnakeCaseNamingConvention();
        });
    }

    public static void AddAuthenticationConfig(this IServiceCollection services, JwtSettings jwtSettings)
    {
        services.AddAuthentication("Bearer")
            .AddJwtBearer("Bearer", options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecurityKey)),
                    RoleClaimType = ClaimTypes.Role
                };
            });
        services.AddAuthorization();
    }

    public static void AddSwaggerConfig(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "API Documentation",
                Version = "v1"
            });

            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Enter JWT token like this: Bearer {your token}"
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[] {}
                }
            });
        });
    }

    private static void AddCorsConfig(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("devCors",builder =>
            {
                builder.AllowAnyOrigin();
                builder.AllowAnyHeader();
                builder.AllowAnyMethod();
            });
        });
    }
}