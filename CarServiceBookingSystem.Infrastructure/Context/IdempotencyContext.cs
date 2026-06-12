using Microsoft.AspNetCore.Http;

public class IdempotencyContext : IIdempotencyContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public IdempotencyContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? Key =>
        _httpContextAccessor.HttpContext?
            .Request
            .Headers["Idempotency-Key"]
            .ToString();
}