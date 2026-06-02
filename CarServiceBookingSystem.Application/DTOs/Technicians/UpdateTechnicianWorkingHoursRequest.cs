namespace CarServiceBookingSystem.Application.DTOs.Technicians;

public class UpdateTechnicianWorkingHoursRequest
{
    public List<UpdateTechnicianWorkingHourRequest> WorkingHours { get; set; } = new();
}