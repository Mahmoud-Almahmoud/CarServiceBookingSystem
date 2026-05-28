namespace CarServiceBookingSystem.Application.Security;

public static class Permissions
{
    public static class Services
    {
        public const string View = "Services.View";
        public const string Create = "Services.Create";
        public const string Update = "Services.Update";
        public const string Delete = "Services.Delete";
    }

    public static class Bookings
    {
        public const string ViewMine = "Bookings.ViewMine";
        public const string ViewAll = "Bookings.ViewAll";
        public const string Create = "Bookings.Create";
        public const string UpdateStatus = "Bookings.UpdateStatus";
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
    }

    public static class ApiKeys
    {
        public const string View = "ApiKeys.View";
        public const string Create = "ApiKeys.Create";
        public const string Revoke = "ApiKeys.Revoke";
    }

    public static class Maintenance
    {
        public const string Manage = "maintenance.manage";
    }
}