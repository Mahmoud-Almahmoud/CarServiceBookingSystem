using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;

namespace CarServiceBookingSystem.API.Middleware;

public sealed class SwaggerBasicAuthMiddleware
{
    private readonly RequestDelegate _next;

    public SwaggerBasicAuthMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(
        HttpContext context,
        IConfiguration configuration,
        IHostEnvironment environment)
    {
        if (!context.Request.Path.StartsWithSegments("/swagger"))
        {
            await _next(context);
            return;
        }

        //await _next(context);
        //return;

        //Allow Swagger freely in local development.
        if (environment.IsDevelopment())
        {
            await _next(context);
            return;
        }

        var username = configuration["SwaggerAuth:Username"];
        var password = configuration["SwaggerAuth:Password"];

        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsync("Swagger authentication is not configured.");
            return;
        }

        if (!context.Request.Headers.TryGetValue("Authorization", out var authorizationHeader))
        {
            Challenge(context);
            return;
        }

        if (!AuthenticationHeaderValue.TryParse(authorizationHeader, out var authHeader))
        {
            Challenge(context);
            return;
        }

        if (!"Basic".Equals(authHeader.Scheme, StringComparison.OrdinalIgnoreCase))
        {
            Challenge(context);
            return;
        }

        if (string.IsNullOrWhiteSpace(authHeader.Parameter))
        {
            Challenge(context);
            return;
        }

        string decodedCredentials;

        try
        {
            var credentialBytes = Convert.FromBase64String(authHeader.Parameter);
            decodedCredentials = Encoding.UTF8.GetString(credentialBytes);
        }
        catch
        {
            Challenge(context);
            return;
        }

        var separatorIndex = decodedCredentials.IndexOf(':');

        if (separatorIndex <= 0)
        {
            Challenge(context);
            return;
        }

        var providedUsername = decodedCredentials[..separatorIndex];
        var providedPassword = decodedCredentials[(separatorIndex + 1)..];

        if (!SecureEquals(providedUsername, username) ||
            !SecureEquals(providedPassword, password))
        {
            Challenge(context);
            return;
        }

        await _next(context);
    }

    private static void Challenge(HttpContext context)
    {
        context.Response.Headers.WWWAuthenticate = "Basic realm=\"Swagger\"";
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
    }

    private static bool SecureEquals(string value, string expected)
    {
        var valueBytes = Encoding.UTF8.GetBytes(value);
        var expectedBytes = Encoding.UTF8.GetBytes(expected);

        return valueBytes.Length == expectedBytes.Length &&
               CryptographicOperations.FixedTimeEquals(valueBytes, expectedBytes);
    }
}