using System.Net;
using System.Text.Json;

namespace BadNews.Middleware;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
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
            _logger.LogError(ex, "An unhandled exception occurred");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var response = new ErrorResponse();

        switch (exception)
        {
            case ArgumentNullException ex:
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                response = new ErrorResponse
                {
                    StatusCode = 400,
                    Message = "Invalid input",
                    Details = ex.Message
                };
                break;

            case UnauthorizedAccessException ex:
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                response = new ErrorResponse
                {
                    StatusCode = 401,
                    Message = "Unauthorized",
                    Details = ex.Message
                };
                break;

            case KeyNotFoundException ex:
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                response = new ErrorResponse
                {
                    StatusCode = 404,
                    Message = "Resource not found",
                    Details = ex.Message
                };
                break;

            case InvalidOperationException ex:
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                response = new ErrorResponse
                {
                    StatusCode = 400,
                    Message = "Invalid operation",
                    Details = ex.Message
                };
                break;

            default:
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                response = new ErrorResponse
                {
                    StatusCode = 500,
                    Message = "Internal server error",
                    Details = exception.Message
                };
                break;
        }

        return context.Response.WriteAsJsonAsync(response);
    }
}

public class ErrorResponse
{
    public int StatusCode { get; set; }
    public string Message { get; set; } = null!;
    public string? Details { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

public static class ErrorHandlingExtensions
{
    public static IApplicationBuilder UseErrorHandling(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ErrorHandlingMiddleware>();
    }
}
