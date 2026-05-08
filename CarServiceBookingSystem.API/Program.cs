using Asp.Versioning;
using CarServiceBookingSystem.API;
using CarServiceBookingSystem.API.Filters;
using CarServiceBookingSystem.API.Middlewares;
using CarServiceBookingSystem.Infrastructure.Identity;
using CarServiceBookingSystem.Infrastructure.Persistence;
using Hangfire;
using Microsoft.AspNetCore.Identity;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File(
        "Logs/log-.txt",
        rollingInterval: RollingInterval.Day)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();

builder.Services.AddAPIDependencies(builder.Configuration);

var app = builder.Build();
app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseSerilogRequestLogging();



if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization =
    [
        new HangfireAdminAuthorizationFilter()
    ]
});

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider
        .GetRequiredService<ApplicationDbContext>();

    var userManager = scope.ServiceProvider
        .GetRequiredService<UserManager<ApplicationUser>>();

    var roleManager = scope.ServiceProvider
        .GetRequiredService<RoleManager<IdentityRole>>();

    await DbSeeder.SeedAsync(dbContext, userManager, roleManager);
    await CarLookupSeeder.SeedAsync(dbContext);
}

app.Run();
