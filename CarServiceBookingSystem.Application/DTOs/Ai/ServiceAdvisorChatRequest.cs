using System.ComponentModel.DataAnnotations;

namespace CarServiceBookingSystem.Application.DTOs.Ai;

public sealed class ServiceAdvisorChatRequest
{
    [Required]
    [StringLength(2000, MinimumLength = 3)]
    public string Message { get; init; } = string.Empty;

    public int? CarId { get; init; }

    public List<AiChatMessageDto> History { get; init; } = [];
}

public sealed class AiChatMessageDto
{
    [Required]
    public string Role { get; init; } = string.Empty;

    [Required]
    [StringLength(2000)]
    public string Content { get; init; } = string.Empty;
}