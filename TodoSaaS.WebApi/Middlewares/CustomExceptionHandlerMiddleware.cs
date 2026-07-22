using System.Text.Json;
using TodoSaaS.Application.Common.Exceptions;

namespace TodoSaaS.WebApi.Middlewares;

public class CustomExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;

    public CustomExceptionHandlerMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch(Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }
            private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var code = StatusCodes.Status500InternalServerError;
            var result = string.Empty;
    
            // Si la excepción es nuestra excepción de validaciones personalizadas
            if (exception is ValidationException validationException)
            {
                code = StatusCodes.Status400BadRequest;
                result = JsonSerializer.Serialize(new { 
                    title = "Ocurrieron uno o más errores de validación.",
                    status = code,
                    errors = validationException.Errors 
                });
            }
            else
            {
                result = JsonSerializer.Serialize(new { 
                    title = "Error interno del servidor.", 
                    status = code,
                    detail = exception.Message 
                });
            }
    
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = code;
    
            return context.Response.WriteAsync(result);
        }
    
}