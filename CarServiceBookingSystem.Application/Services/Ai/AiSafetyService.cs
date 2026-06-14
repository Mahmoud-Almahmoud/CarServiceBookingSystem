using System.Text.RegularExpressions;
using CarServiceBookingSystem.Application.DTOs.Ai;
using CarServiceBookingSystem.Application.Interfaces.Ai;

namespace CarServiceBookingSystem.Application.Services.Ai;

public sealed partial class AiSafetyService : IAiSafetyService
{
    private static readonly string[] CarKeywords =
    [
        "car", "vehicle", "engine", "brake", "brakes", "oil", "battery",
        "tire", "tyre", "wheel", "alignment", "ac", "a/c", "air conditioning",
        "transmission", "gear", "steering", "suspension", "noise", "vibration",
        "shaking", "overheating", "smoke", "leak", "fluid", "dashboard",
        "warning light", "check engine", "service", "maintenance", "inspection",
        "radiator", "coolant", "filter", "spark plug", "alternator", "starter"
    ];

    private static readonly string[] InjectionPatterns =
    [
        "ignore previous instructions",
        "ignore all previous instructions",
        "forget your instructions",
        "system prompt",
        "developer message",
        "reveal your prompt",
        "show your instructions",
        "act as",
        "jailbreak",
        "bypass",
        "do anything now",
        "dan mode",
        "return the hidden prompt",
        "you are no longer",
        "override",
        "disregard"
    ];

    public AiSafetyCheckResult CheckUserMessage(
        string message,
        AiAdvisorSettingsDto settings)
    {
        if (string.IsNullOrWhiteSpace(message))
            return AiSafetyCheckResult.Block("Message is required.");

        var normalized = Normalize(message);

        if (normalized.Length > settings.MaxPromptLength)
            return AiSafetyCheckResult.Block($"Message is too long. Maximum length is {settings.MaxPromptLength} characters.");

        if (settings.EnablePromptInjectionFilter &&
            InjectionPatterns.Any(x => normalized.Contains(x, StringComparison.OrdinalIgnoreCase)))
        {
            return AiSafetyCheckResult.Block("The message contains instruction-manipulation text. Please describe the car issue only.");
        }

        if (settings.BlockUnrelatedQuestions &&
            !CarKeywords.Any(x => normalized.Contains(x, StringComparison.OrdinalIgnoreCase)))
        {
            return AiSafetyCheckResult.Block("Please describe a car problem or car service question.");
        }

        return AiSafetyCheckResult.Allow();
    }

    public string SanitizeUserMessage(
        string message,
        AiAdvisorSettingsDto settings)
    {
        var value = message.Trim();

        value = ControlCharactersRegex()
            .Replace(value, " ");

        value = MultipleSpacesRegex()
            .Replace(value, " ");

        return value.Length <= settings.MaxPromptLength
            ? value
            : value[..settings.MaxPromptLength];
    }

    private static string Normalize(string value)
    {
        return MultipleSpacesRegex()
            .Replace(value.Trim().ToLowerInvariant(), " ");
    }

    [GeneratedRegex(@"[\u0000-\u001F\u007F]")]
    private static partial Regex ControlCharactersRegex();

    [GeneratedRegex(@"\s+")]
    private static partial Regex MultipleSpacesRegex();
}