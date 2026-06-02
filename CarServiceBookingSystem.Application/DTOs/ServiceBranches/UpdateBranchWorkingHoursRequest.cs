namespace CarServiceBookingSystem.Application.DTOs.ServiceBranches;

public class UpdateBranchWorkingHoursRequest
{
    public List<UpsertBranchWorkingHourRequest> WorkingHours { get; set; } = [];
}