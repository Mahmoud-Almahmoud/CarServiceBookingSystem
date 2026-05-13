using CarServiceBookingSystem.Application.Common;
using System.Net;
using System.Text.Json;

namespace CarServiceBookingSystem.API.Middlewares;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionMiddleware> logger)
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
            _logger.LogError(ex, "Unhandled exception occurred");

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var isTesting = context.RequestServices
                .GetRequiredService<IWebHostEnvironment>()
                .IsEnvironment("Testing");

            var errors = isTesting
                ? new List<string>
                {
            ex.Message,
            ex.StackTrace ?? string.Empty
                }
                : new List<string> { ex.Message };

            var response = ApiResponse<string>.Fail(
                "An unexpected error occurred",
                errors);

            var json = JsonSerializer.Serialize(response);

            await context.Response.WriteAsync(json);
        }
    }
}