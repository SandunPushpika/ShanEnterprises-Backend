using System.Net;
using System.Text.Json;
using Core.DTOs.Response;
using Core.Exceptions;
using Core.Exceptions.Auth;
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
        catch (UnauthorizedUserException ex)
        {
            await HandleUnAuthorizedExceptionAsync(context);
        }
        catch (UnauthorizedAccessException ex)
        {
            await HandleUnAuthorizedExceptionAsync(context);
        }
        catch (NotFoundException ex)
        {
            await HandleNotFoundExceptionAsync(context, ex.Message);
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
        var serializedMessage = JsonSerializer.Serialize(message, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int) HttpStatusCode.BadRequest;
        await context.Response.WriteAsync(serializedMessage);
    }

    private async Task HandleUnAuthorizedExceptionAsync(HttpContext context)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int) HttpStatusCode.Unauthorized;

        var serializedMessage = JsonSerializer.Serialize(new ApiResponse("Please login again to continue", false), new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        await context.Response.WriteAsync(serializedMessage);
    }
    
    private async Task HandleNotFoundExceptionAsync(HttpContext context, string errorMessage)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int) HttpStatusCode.NotFound;

        var serializedMessage = JsonSerializer.Serialize(new ApiResponse(errorMessage, false), new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        await context.Response.WriteAsync(serializedMessage);
    }
}