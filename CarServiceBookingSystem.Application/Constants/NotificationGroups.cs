namespace CarServiceBookingSystem.Application.Constants;

public static class NotificationGroups
{
    public const string Admins = "admins";
    public const string Technicians = "technicians";

    public static string BranchAdmins(int branchId)
    {
        return $"branch-{branchId}-admins";
    }

    public static string Booking(int bookingId)
    {
        return $"booking-{bookingId}";
    }
}