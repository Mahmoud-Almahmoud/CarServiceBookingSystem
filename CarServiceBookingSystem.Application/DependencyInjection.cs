using CarServiceBookingSystem.Application.Interfaces.Ai;
using CarServiceBookingSystem.Application.Interfaces.IAi;
using CarServiceBookingSystem.Application.Services.Ai;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace CarServiceBookingSystem.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        services.AddScoped<IAiServiceAdvisorService, AiServiceAdvisorService>();
        services.AddScoped<IAiConversationService, AiConversationService>();
        services.AddScoped<IAiRecommendationFeedbackService, AiRecommendationFeedbackService>();
        services.AddScoped<IAiAnalyticsService, AiAnalyticsService>();
        services.AddScoped<IAiSafetyService, AiSafetyService>();

        return services;
    }
}