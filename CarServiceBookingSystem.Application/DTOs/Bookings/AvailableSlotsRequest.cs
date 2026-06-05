using CarServiceBookingSystem.Domain.Enums;

namespace CarServiceBookingSystem.Application.DTOs.Bookings;

public class AvailableSlotsRequest
{
    public DateTime Date { get; set; }

    public int ServiceId { get; set; }
    public int ServiceBranchId { get; set; }

    public ServiceLocationType LocationType { get; set; }
}