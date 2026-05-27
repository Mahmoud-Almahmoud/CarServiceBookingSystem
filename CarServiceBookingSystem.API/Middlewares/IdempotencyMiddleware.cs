using CarServiceBookingSystem.Application.Common.Interfaces;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace CarServiceBookingSystem.API.Middleware;

public class IdempotencyMiddleware
{
    private const string HeaderName = "Idempotency-Key";

    private readonly RequestDelegate _next;

    public IdempotencyMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(
        HttpContext context,
        IIdempotencyService idempotencyService)
    {
        if (!RequiresIdempotency(context))
        {
            await _next(context);
            return;
        }

        var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(userId))
        {
            await _next(context);
            return;
        }

        if (!context.Request.Headers.TryGetValue(HeaderName, out var keyValues))
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsJsonAsync(new
            {
                message = "Idempotency-Key header is required."
            });
            return;
        }

        var key = keyValues.ToString();

        if (string.IsNullOrWhiteSpace(key) || key.Length > 200)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsJsonAsync(new
            {
                message = "Invalid Idempotency-Key header."
            });
            return;
        }

        context.Request.EnableBuffering();

        var requestBody = await ReadRequestBodyAsync(context.Request);
        var hashInput =
            $"{context.Request.Method}|" +
            $"{context.Request.Path.Value?.ToLowerInvariant()}|" +
            $"{context.Request.QueryString.Value?.ToLowerInvariant()}|" +
            $"{requestBody}";

        var requestHash = ComputeSha256Hash(hashInput);

        var endpoint = $"{context.Request.Method}:{context.Request.Path.Value?.ToLowerInvariant()}";

        var check = await idempotencyService.CheckAsync(
            key,
            userId,
            endpoint,
            requestHash,
            context.RequestAborted);

        if (check.IsConflict)
        {
            context.Response.StatusCode = StatusCodes.Status409Conflict;
            await context.Response.WriteAsJsonAsync(new
            {
                message = "Idempotency key is already used for a different or pending request."
            });
            return;
        }

        if (check.IsReplay)
        {
            context.Response.StatusCode = check.StatusCode ?? StatusCodes.Status200OK;
            context.Response.ContentType = "application/json";

            if (!string.IsNullOrWhiteSpace(check.ResponseBody))
            {
                await context.Response.WriteAsync(check.ResponseBody);
            }

            return;
        }

        var originalBody = context.Response.Body;

        await using var memoryStream = new MemoryStream();
        context.Response.Body = memoryStream;

        await _next(context);

        memoryStream.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(memoryStream).ReadToEndAsync();

        if (check.RecordId.HasValue && context.Response.StatusCode < 500)
        {
            await idempotencyService.CompleteAsync(
                check.RecordId.Value,
                context.Response.StatusCode,
                responseBody,
                context.RequestAborted);
        }

        memoryStream.Seek(0, SeekOrigin.Begin);
        await memoryStream.CopyToAsync(originalBody);

        context.Response.Body = originalBody;
    }

    private static bool RequiresIdempotency(HttpContext context)
    {
        if (!HttpMethods.IsPost(context.Request.Method))
            return false;

        var path = context.Request.Path.Value?.ToLowerInvariant();

        return path is not null &&
               (
                   path.EndsWith("/bookings") ||
                   Regex.IsMatch(path, @"^/api/v\d+(\.\d+)?/payments/create-intent/\d+$")
               );
    }

    private static async Task<string> ReadRequestBodyAsync(HttpRequest request)
    {
        request.Body.Position = 0;

        using var reader = new StreamReader(
            request.Body,
            Encoding.UTF8,
            detectEncodingFromByteOrderMarks: false,
            leaveOpen: true);

        var body = await reader.ReadToEndAsync();

        request.Body.Position = 0;

        return body;
    }

    private static string ComputeSha256Hash(string input)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(input));
        return Convert.ToHexString(bytes);
    }
}