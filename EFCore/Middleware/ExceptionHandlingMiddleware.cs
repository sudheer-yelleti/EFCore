namespace EFCore.Middleware;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
public class ExceptionHandlingMiddleware: IMiddleware
{
    
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(ILogger<ExceptionHandlingMiddleware> logger)
    {
        _logger = logger;
    }
    
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred while processing the request.");
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            var errorResponse = new
            {
                Message = "An unexpected error occurred."+ ex.Message,
                StatusCode = context.Response.StatusCode
            };
            await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(errorResponse));
        }
    }
}