using Auth10Api.Application.Common;
using System.Net;
using System.Text.Json;

namespace Auth10Api.Infrastructure.Middleware;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(RequestDelegate next,
                                   ILogger<ErrorHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Critical Error");

            context.Response.ContentType = "application/json";

            context.Response.StatusCode = 500;

            var response = new Result<object>
            {
                Success = false,
                Message = "Internal Server Error.",
                Errors = ex.ToString() 
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var code = HttpStatusCode.InternalServerError;
        var result = JsonSerializer.Serialize(new { error = "An internal error occurred. Please try again later." });

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)code;

        return context.Response.WriteAsync(result);
    }
}
