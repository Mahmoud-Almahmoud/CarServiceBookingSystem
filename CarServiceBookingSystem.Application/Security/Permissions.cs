namespace CarServiceBookingSystem.Application.Security;

public static class Permissions
{
    public static class Services
    {
        public const string View = "Services.View";
        public const string Create = "Services.Create";
        public const string Update = "Services.Update";
        public const string Delete = "Services.Delete";
        public const string Manage = "Services.Manage";
    }

    public static class Bookings
    {
        public const string ViewMine = "Bookings.ViewMine";
        public const string ViewAll = "Bookings.ViewAll";
        public const string Create = "Bookings.Create";
        public const string UpdateStatus = "Bookings.UpdateStatus";
        public const string Manage = "Bookings.Manage";
    }

    public static class Payments
    {
        public const string View = "Payments.View";
        public const string Create = "Payments.Create";
        public const string Manage = "Payments.Manage";
    }

    public static class Cars
    {
        public const string ViewMine = "Cars.ViewMine";
        public const string Create = "Cars.Create";
        public const string Update = "Cars.Update";
        public const string Delete = "Cars.Delete";
    }

    public static class SecurityAudit
    {
        public const string ViewAll = "SecurityAudit.ViewAll";
        public const string ViewMine = "SecurityAudit.ViewMine";
    }

    public static class Users
    {
        public const string Manage = "Users.Manage";
        public const string ManageMine = "Users.ManageMine";
        public const string View = "Users.View";
    }

    public static class ApiKeys
    {
        public const string View = "ApiKeys.View";
        public const string Create = "ApiKeys.Create";
        public const string Revoke = "ApiKeys.Revoke";
        public const string ViewUsage = "ApiKeys.ViewUsage";
    }

    public static class Maintenance
    {
        public const string Manage = "maintenance.manage";
    }

    public static class System
    {
        public const string View = "system.view";
    }

    public static class Roles
    {
        public const string View = "Roles.View";
        public const string Create = "Roles.Create";
        public const string Update = "Roles.Update";
        public const string Delete = "Roles.Delete";
        public const string ManagePermissions = "Roles.ManagePermissions";
    }

    public static class PermissionsCatalog
    {
        public const string View = "Permissions.View";
    }

    public static class BackgroundJobs
    {
        public const string View = "BackgroundJobs.View";
    }

    public static class Technicians
    {
        public const string View = "Technicians.View";
        public const string Manage = "Technicians.Manage";
    }

    public static class Notifications
    {
        public const string Send = "Notifications.Send";
        public const string ViewAll = "Notifications.ViewAll";
    }
}