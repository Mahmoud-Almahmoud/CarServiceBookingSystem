namespace CarServiceBookingSystem.Domain.Entities;

public class Technician :BaseIdEntity
{
    public int ServiceBranchId { get; set; }
    public ServiceBranch ServiceBranch { get; set; } = null!;

    public string FullName { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }

    public bool IsActive { get; set; } = true;

    public ICollection<TechnicianService> TechnicianServices { get; set; } = new List<TechnicianService>();
    public ICollection<TechnicianWorkingHour> WorkingHours { get; set; } = new List<TechnicianWorkingHour>();
    public ICollection<TechnicianUnavailableDate> UnavailableDates { get; set; } = new List<TechnicianUnavailableDate>();
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();

}