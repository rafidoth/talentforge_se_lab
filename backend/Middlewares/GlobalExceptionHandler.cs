using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics;
using server.Exceptions;

namespace server.Middlewares;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "An unhandled exception occurred");

        var statusCode = exception switch
        {
            AppException appEx => appEx.StatusCode,
            _ => StatusCodes.Status500InternalServerError
        };

        var response = new
        {
            Title = "An error occurred",
            Message = exception.Message,
            StatusCode = statusCode
        };

        httpContext.Response.StatusCode = statusCode;
        httpContext.Response.ContentType = "application/json";

        await httpContext.Response.WriteAsync(JsonSerializer.Serialize(response), cancellationToken);

        return true;
    }
}
