using CarServiceBookingSystem.Application.Security;
using CarServiceBookingSystem.Domain.Entities;
using CarServiceBookingSystem.Domain.Enums;
using CarServiceBookingSystem.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

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

        // Admin permissions
        await AddPermissionClaimAsync(roleManager, Roles.Admin, Permissions.Services.View);
        await AddPermissionClaimAsync(roleManager, Roles.Admin, Permissions.Services.Create);
        await AddPermissionClaimAsync(roleManager, Roles.Admin, Permissions.Services.Update);
        await AddPermissionClaimAsync(roleManager, Roles.Admin, Permissions.Services.Delete);
        await AddPermissionClaimAsync(roleManager, Roles.Admin, Permissions.Services.Manage);

        await AddPermissionClaimAsync(roleManager, Roles.Admin, Permissions.Bookings.ViewAll);
        await AddPermissionClaimAsync(roleManager, Roles.Admin, Permissions.Bookings.UpdateStatus);
        await AddPermissionClaimAsync(roleManager, Roles.Admin, Permissions.Bookings.Manage);

        await AddPermissionClaimAsync(roleManager, Roles.Admin, Permissions.SecurityAudit.ViewAll);

        await AddPermissionClaimAsync(roleManager, Roles.Admin, Permissions.Users.Manage);
        await AddPermissionClaimAsync(roleManager, Roles.Admin, Permissions.Users.View);

        await AddPermissionClaimAsync(roleManager, Roles.Admin, Permissions.ApiKeys.Create);
        await AddPermissionClaimAsync(roleManager, Roles.Admin, Permissions.ApiKeys.Revoke);
        await AddPermissionClaimAsync(roleManager, Roles.Admin, Permissions.ApiKeys.View);
        await AddPermissionClaimAsync(roleManager, Roles.Admin, Permissions.ApiKeys.ViewUsage);


        await AddPermissionClaimAsync(roleManager, Roles.Admin, Permissions.Maintenance.Manage);

        await AddPermissionClaimAsync(roleManager, Roles.Admin, Permissions.System.View);

        await AddPermissionClaimAsync(roleManager, Roles.Admin, Permissions.Roles.View);
        await AddPermissionClaimAsync(roleManager, Roles.Admin, Permissions.Roles.Create);
        await AddPermissionClaimAsync(roleManager, Roles.Admin, Permissions.Roles.Update);
        await AddPermissionClaimAsync(roleManager, Roles.Admin, Permissions.Roles.Delete);
        await AddPermissionClaimAsync(roleManager, Roles.Admin, Permissions.Roles.ManagePermissions);

        await AddPermissionClaimAsync(roleManager, Roles.Admin, Permissions.PermissionsCatalog.View);

        await AddPermissionClaimAsync(roleManager, Roles.Admin, Permissions.BackgroundJobs.View);

        // User permissions
        await AddPermissionClaimAsync(roleManager, Roles.User, Permissions.Services.View);

        await AddPermissionClaimAsync(roleManager, Roles.User, Permissions.Bookings.ViewMine);
        await AddPermissionClaimAsync(roleManager, Roles.User, Permissions.Bookings.Create);

        await AddPermissionClaimAsync(roleManager, Roles.User, Permissions.Cars.ViewMine);
        await AddPermissionClaimAsync(roleManager, Roles.User, Permissions.Cars.Create);
        await AddPermissionClaimAsync(roleManager, Roles.User, Permissions.Cars.Update);

        await AddPermissionClaimAsync(roleManager, Roles.User, Permissions.SecurityAudit.ViewMine);
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

    private static async Task AddPermissionClaimAsync(
    RoleManager<IdentityRole> roleManager,
    string roleName,
    string permission)
    {
        var role = await roleManager.FindByNameAsync(roleName);

        if (role == null)
            return;

        var claims = await roleManager.GetClaimsAsync(role);

        if (claims.Any(x => x.Type == CustomClaimTypes.Permission && x.Value == permission))
            return;

        await roleManager.AddClaimAsync(
            role,
            new Claim(CustomClaimTypes.Permission, permission));
    }
}