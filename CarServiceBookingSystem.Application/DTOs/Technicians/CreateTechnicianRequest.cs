namespace CarServiceBookingSystem.Application.DTOs.Technicians;

public class CreateTechnicianRequest
{
    public int ServiceBranchId { get; set; }

    public string FullName { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }

    public bool IsActive { get; set; } = true;
}