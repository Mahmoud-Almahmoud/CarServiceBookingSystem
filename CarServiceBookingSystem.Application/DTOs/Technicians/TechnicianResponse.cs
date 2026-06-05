namespace CarServiceBookingSystem.Application.DTOs.Technicians;

public class TechnicianResponse
{
    public int Id { get; set; }
    public int ServiceBranchId { get; set; }
    public string ServiceBranchName { get; set; } = string.Empty;

    public string FullName { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }

    public bool IsActive { get; set; }

    public List<TechnicianServiceResponse> Services { get; set; } = new();

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}