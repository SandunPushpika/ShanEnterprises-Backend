using System.Net;
using System.Text.Json;
using Core.DTOs.Response;
using Exception = System.Exception;

namespace ShanEnterprises.Configs.Middlewares;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }
    
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var message = new ApiResponse(exception.Message, false);
        var serializedMessage = JsonSerializer.Serialize(message);
        
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int) HttpStatusCode.BadRequest;
        await context.Response.WriteAsync(serializedMessage);
    }
}