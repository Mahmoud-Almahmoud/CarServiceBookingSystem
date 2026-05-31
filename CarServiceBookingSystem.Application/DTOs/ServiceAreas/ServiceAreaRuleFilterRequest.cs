using CarServiceBookingSystem.Application.Common;

namespace CarServiceBookingSystem.Application.DTOs.ServiceAreas;

public class ServiceAreaRuleFilterRequest :PagedRequest
{
    public int? ServiceId { get; set; }

    public string? CountryCode { get; set; }

    public string? City { get; set; }

    public bool? IsAllowed { get; set; }

    public bool? IsActive { get; set; }

}