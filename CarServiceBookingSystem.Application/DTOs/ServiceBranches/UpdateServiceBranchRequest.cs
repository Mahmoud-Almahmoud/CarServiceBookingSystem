namespace CarServiceBookingSystem.Application.DTOs.ServiceBranches;

public class UpdateServiceBranchRequest
{
    public string Name { get; set; } = string.Empty;

    public string CountryCode { get; set; } = string.Empty;

    public string City { get; set; } = string.Empty;

    public decimal Latitude { get; set; }

    public decimal Longitude { get; set; }

    public bool IsActive { get; set; }
}