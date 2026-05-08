using Hangfire.Dashboard;

namespace CarServiceBookingSystem.API.Filters;

public class HangfireAdminAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        var httpContext = context.GetHttpContext();

        if (httpContext.RequestServices
            .GetRequiredService<IWebHostEnvironment>()
            .IsDevelopment())
        {
            return true;
        }

        return httpContext.User.Identity?.IsAuthenticated == true &&
               httpContext.User.IsInRole("Admin");
    }
}