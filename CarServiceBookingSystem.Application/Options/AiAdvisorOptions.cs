namespace CarServiceBookingSystem.Application.Options;

public sealed class AiAdvisorOptions
{
    public bool Enabled { get; set; } = true;

    public string Provider { get; set; } = "Ollama";

    public string BaseUrl { get; set; } = "http://localhost:11434";

    public string Model { get; set; } = "llama3.1:8b";

    public int TimeoutSeconds { get; set; } = 60;

    public int MaxSuggestions { get; set; } = 3;

    public string BookingPathTemplate { get; set; } = "/app/bookings/new?serviceId={0}";
}