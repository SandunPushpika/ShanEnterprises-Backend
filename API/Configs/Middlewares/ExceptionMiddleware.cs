using System.Net;
using System.Text.Json;
using Core.DTOs.Response;
using FluentValidation;
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
        catch (ValidationException ex)
        {
            await HandleExceptionAsync(context, ex.Errors.FirstOrDefault()?.ErrorMessage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            await HandleExceptionAsync(context, ex.Message);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, string? errorMessage)
    {
        var message = new ApiResponse(errorMessage, false);
        var serializedMessage = JsonSerializer.Serialize(message);
        
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int) HttpStatusCode.BadRequest;
        await context.Response.WriteAsync(serializedMessage);
    }
}