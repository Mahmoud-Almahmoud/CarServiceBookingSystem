using CarServiceBookingSystem.Domain.Entities;
using CarServiceBookingSystem.Domain.Enums;
using CarServiceBookingSystem.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CarServiceBookingSystem.Infrastructure.Persistence;

public static class DbSeeder
{
    public static async Task SeedAsync(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        await SeedRolesAsync(roleManager);
        await SeedAdminUserAsync(userManager);
        await SeedServicesAsync(context);
    }

    private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
    {
        if (!await roleManager.RoleExistsAsync(Roles.Admin))
            await roleManager.CreateAsync(new IdentityRole(Roles.Admin));

        if (!await roleManager.RoleExistsAsync(Roles.User))
            await roleManager.CreateAsync(new IdentityRole(Roles.User));
    }

    private static async Task SeedAdminUserAsync(UserManager<ApplicationUser> userManager)
    {
        const string adminEmail = "admin@carservice.com";
        const string adminPassword = "Admin123!";

        var admin = await userManager.FindByEmailAsync(adminEmail);

        if (admin == null)
        {
            admin = new ApplicationUser
            {
                FullName = "System Admin",
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(admin, adminPassword);

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(admin, Roles.Admin);
            }
        }
    }

    private static async Task SeedServicesAsync(ApplicationDbContext context)
    {
        if (await context.Services.AnyAsync())
            return;

        var services = new List<Service>
        {
            new()
            {
                Name = "Oil Change",
                Price = 120,
                DurationInMinutes = 30
            },
            new()
            {
                Name = "Full Car Wash",
                Price = 80,
                DurationInMinutes = 45
            },
            new()
            {
                Name = "Brake Inspection",
                Price = 150,
                DurationInMinutes = 60
            },
            new()
            {
                Name = "Battery Check",
                Price = 50,
                DurationInMinutes = 20
            }
        };

        await context.Services.AddRangeAsync(services);
        await context.SaveChangesAsync();
    }
}