namespace CarServiceBookingSystem.Application.DTOs.ServiceBranches;

public class AddBranchServiceRequest
{
    public int ServiceId { get; set; }

    public bool IsActive { get; set; } = true;
}