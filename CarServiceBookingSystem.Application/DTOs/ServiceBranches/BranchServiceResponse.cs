namespace CarServiceBookingSystem.Application.DTOs.ServiceBranches;

public class BranchServiceResponse
{
    public int Id { get; set; }

    public int ServiceBranchId { get; set; }

    public string BranchName { get; set; } = string.Empty;

    public int ServiceId { get; set; }

    public string ServiceName { get; set; } = string.Empty;

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}