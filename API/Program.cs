using Core.Helpers;
using DotNetEnv;
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

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDatabaseConfig(appSettings.DefaultConnection);
builder.Services.AddRequiredServices();
builder.Services.AddControllers().ConfigureControllers();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionMiddleware>();
app.UseHttpsRedirection();

app.MapControllers();

app.Run();