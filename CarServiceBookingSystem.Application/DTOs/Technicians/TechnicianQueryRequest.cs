using CarServiceBookingSystem.Application.Common;

namespace CarServiceBookingSystem.Application.DTOs.Technicians;

public class TechnicianQueryRequest : PagedRequest
{
    public int? ServiceBranchId { get; set; }
    public int? ServiceId { get; set; }
    public bool? IsActive { get; set; }
}