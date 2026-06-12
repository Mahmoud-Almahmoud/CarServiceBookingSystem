using CarServiceBookingSystem.Application.Common;
using CarServiceBookingSystem.Application.Interfaces.IAuth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CarServiceBookingSystem.API.Auth;

public class ApiKeyAuthorizeAttribute : Attribute, IAsyncAuthorizationFilter
{
    private const string ApiKeyHeaderName = "X-API-Key";

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var apiKeyService = context.HttpContext.RequestServices
            .GetRequiredService<IApiKeyService>();

        if (!context.HttpContext.Request.Headers.TryGetValue(ApiKeyHeaderName, out var extractedApiKey))
        {
            context.Result = new UnauthorizedObjectResult(
                ApiResponse<string>.Fail("API key is missing"));

            return;
        }

        var ipAddress = context.HttpContext.Connection.RemoteIpAddress?.ToString();

        var isValid = await apiKeyService.ValidateAsync(
            extractedApiKey.ToString(),
            ipAddress);

        if (!isValid)
        {
            context.Result = new UnauthorizedObjectResult(
                ApiResponse<string>.Fail("Invalid API key"));
        }
    }
}