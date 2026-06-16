using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using CarServiceBookingSystem.API;
using CarServiceBookingSystem.API.BackgroundJobs;
using CarServiceBookingSystem.API.Filters;
using CarServiceBookingSystem.API.Hubs;
using CarServiceBookingSystem.API.Middleware;
using CarServiceBookingSystem.API.Middlewares;
using CarServiceBookingSystem.Application.Interfaces;
using CarServiceBookingSystem.Application.Interfaces.IBackgrounJobs;
using CarServiceBookingSystem.Infrastructure.Identity;
using CarServiceBookingSystem.Infrastructure.Persistence;
using Hangfire;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Threading.RateLimiting;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File(
        "Logs/log-.txt",
        rollingInterval: RollingInterval.Day)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, services, configuration) =>
{
    configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext();
});
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders =
        ForwardedHeaders.XForwardedFor |
        ForwardedHeaders.XForwardedProto;
});
builder.Services.AddAPIDependencies(builder.Configuration, builder.Environment);


builder.Services.AddHealthChecks();
builder.Services.AddResponseCompression();

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 100,
                Window = TimeSpan.FromMinutes(1),
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 0
            }));

    options.AddPolicy("TwoFactorPolicy", httpContext =>
    RateLimitPartition.GetFixedWindowLimiter(
        partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
        factory: _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = 5,
            Window = TimeSpan.FromMinutes(5),
            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
            QueueLimit = 0
        }));

    options.AddPolicy("AiServiceAdvisorPolicy", httpContext =>
    {
        var userId = httpContext.User.FindFirst("uid")?.Value
            ?? httpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
            ?? httpContext.User.FindFirst("sub")?.Value;

        var anonymousSessionId = httpContext.Request.Headers["X-Anonymous-Session-Id"]
            .FirstOrDefault();

        var ip = httpContext.Connection.RemoteIpAddress?.ToString()
            ?? "unknown-ip";

        var partitionKey = !string.IsNullOrWhiteSpace(userId)
            ? $"user:{userId}"
            : !string.IsNullOrWhiteSpace(anonymousSessionId)
                ? $"anon:{anonymousSessionId}:{ip}"
                : $"ip:{ip}";

        return RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: partitionKey,
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = string.IsNullOrWhiteSpace(userId) ? 5 : 10,
                Window = TimeSpan.FromMinutes(1),
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 0
            });
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendPolicy", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:3000",
                "http://localhost:5173",
                "https://csbs-app.mahmoudev.com",
                "http://csbs-app.mahmoudev.com")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});
var app = builder.Build();

app.UseForwardedHeaders();
if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}
app.Use(async (context, next) =>
{
    context.Response.Headers.TryAdd("X-Content-Type-Options", "nosniff");
    context.Response.Headers.TryAdd("X-Frame-Options", "DENY");
    context.Response.Headers.TryAdd("Referrer-Policy", "no-referrer");
    context.Response.Headers.TryAdd("X-XSS-Protection", "0");
    context.Response.Headers.TryAdd("Permissions-Policy", "geolocation=(), microphone=(), camera=()");
    if (!app.Environment.IsDevelopment())
    {
        context.Response.Headers.TryAdd(
            "Strict-Transport-Security",
            "max-age=31536000; includeSubDomains");
    }

    await next();
});

app.UseResponseCompression();
app.UseRateLimiter();

if (!app.Environment.IsEnvironment("Testing"))
{
    using var scope = app.Services.CreateScope();

    var dbContext = scope.ServiceProvider
        .GetRequiredService<ApplicationDbContext>();

    await dbContext.Database.MigrateAsync();

    var userManager = scope.ServiceProvider
        .GetRequiredService<UserManager<ApplicationUser>>();

    var roleManager = scope.ServiceProvider
        .GetRequiredService<RoleManager<IdentityRole>>();

    var logger = scope.ServiceProvider
        .GetRequiredService<ILoggerFactory>()
        .CreateLogger("PortfolioDemoSeeder");

    await DbSeeder.SeedAsync(dbContext, userManager, roleManager, logger);
}

app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseSerilogRequestLogging();
app.UseMiddleware<SwaggerBasicAuthMiddleware>();

var apiVersionDescriptionProvider =
    app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions)
    {
        options.SwaggerEndpoint(
            $"/swagger/{description.GroupName}/swagger.json",
            $"Car Service Booking API {description.GroupName.ToUpperInvariant()}");
        options.RoutePrefix = "swagger";
    }
});

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseCors("FrontendPolicy");

app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<IdempotencyMiddleware>();

if (!app.Environment.IsEnvironment("Testing"))
{
    app.UseHangfireDashboard("/hangfire", new DashboardOptions
    {
        Authorization =
        [
            new HangfireAdminAuthorizationFilter()
        ]
    });
    RecurringJobsScheduler.RegisterRecurringJobs();
}
app.MapHub<NotificationHub>("/hubs/notifications");
app.MapControllers();
app.MapHealthChecks("/health");

app.Run();
