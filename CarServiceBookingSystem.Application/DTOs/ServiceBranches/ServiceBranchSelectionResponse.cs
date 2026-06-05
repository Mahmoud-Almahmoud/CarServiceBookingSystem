namespace CarServiceBookingSystem.Application.DTOs.ServiceBranches;

public class ServiceBranchSelectionResponse
{
    public int ServiceBranchId { get; set; }

    public string ServiceBranchName { get; set; } = string.Empty;

    public string CountryCode { get; set; } = string.Empty;

    public string City { get; set; } = string.Empty;

    public decimal Latitude { get; set; }

    public decimal Longitude { get; set; }

    public double StraightLineDistanceKm { get; set; }
}