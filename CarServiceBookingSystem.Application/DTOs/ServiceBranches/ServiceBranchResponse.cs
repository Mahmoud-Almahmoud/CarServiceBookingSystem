namespace CarServiceBookingSystem.Application.DTOs.ServiceBranches;

public class ServiceBranchResponse
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string CountryCode { get; set; } = string.Empty;

    public string City { get; set; } = string.Empty;

    public decimal Latitude { get; set; }

    public decimal Longitude { get; set; }

    public bool IsActive { get; set; }

    public int ActiveServicesCount { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}