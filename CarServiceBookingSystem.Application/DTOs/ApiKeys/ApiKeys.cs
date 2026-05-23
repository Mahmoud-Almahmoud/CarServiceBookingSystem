namespace CarServiceBookingSystem.Application.DTOs.ApiKeys;

public class ApiKeyCreatedResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string RawKey { get; set; } = string.Empty;
}