using CarServiceBookingSystem.Application.Common;

namespace CarServiceBookingSystem.Application.DTOs.ServiceBranches;

public class ServiceBranchFilterRequest : PagedRequest
{
    public string? CountryCode { get; set; }

    public string? City { get; set; }

    public int? ServiceId { get; set; }

    public bool? IsActive { get; set; }

}