using Core.Helpers;
using Core.Validators;
using DotNetEnv;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.HttpOverrides;
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

if (File.Exists(envFile))
{
    Env.Load(envFile);
}

var builder = WebApplication.CreateBuilder(args);

var port = Environment.GetEnvironmentVariable("PORT");
if (!string.IsNullOrEmpty(port))
{
    builder.WebHost.UseUrls($"http://0.0.0.0:{port}");
}

builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));
var appSettings = builder.Configuration.GetSection("AppSettings").Get<AppSettings>();

builder.Services.AddSwaggerConfig();
builder.Services.AddDatabaseConfig(appSettings?.DefaultConnection ?? "");
builder.Services.AddRequiredServices();
builder.Services.AddControllers().ConfigureControllers();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<UserUpdateRequestValidator>();
builder.Services.AddAuthenticationConfig(appSettings?.JwtSettings ?? new JwtSettings());
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

var forwardedHeadersOptions = new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
};
forwardedHeadersOptions.KnownIPNetworks.Clear();
forwardedHeadersOptions.KnownProxies.Clear();
app.UseForwardedHeaders(forwardedHeadersOptions);

var enableSwagger = builder.Configuration.GetValue<bool>("EnableSwagger", false) || app.Environment.IsDevelopment();
if (enableSwagger)
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