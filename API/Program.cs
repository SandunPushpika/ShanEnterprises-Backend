using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Application.Repositories;
using Application.Services;
using Core.Helpers;
using Core.Validators;
using DotNetEnv;
using FluentValidation;
using FluentValidation.AspNetCore;
using ShanEnterprises.Configs.Extensions;
using ShanEnterprises.Configs.Middlewares;

var environment =
    Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
    ?? "Development";

var envFile = environment.ToLower() switch
{
    "development" => ".env.development",
    "staging" => ".env.staging",
    "production" => ".env.production",
    _ => ".env"
};

Env.Load(envFile);
var builder = WebApplication.CreateBuilder(args);
builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));
var appSettings = builder.Configuration.GetSection("AppSettings").Get<AppSettings>();


builder.Services.AddSwaggerConfig();
builder.Services.AddDatabaseConfig(appSettings.DefaultConnection);
builder.Services.AddRequiredServices();
builder.Services.AddControllers().ConfigureControllers();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<UserUpdateRequestValidator>();
builder.Services.AddAuthenticationConfig(appSettings.JwtSettings);
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IBookingRepository, BookingRepository>();
builder.Services.AddScoped<IBookingService, BookingService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("devCors");

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<ExceptionMiddleware>();

app.MapControllers();

app.Run();