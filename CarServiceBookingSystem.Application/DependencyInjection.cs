using CarServiceBookingSystem.Application.DTOs.Auth;
using CarServiceBookingSystem.Application.DTOs.Bookings;
using CarServiceBookingSystem.Application.DTOs.Cars;
using CarServiceBookingSystem.Application.DTOs.Services;
using CarServiceBookingSystem.Application.Interfaces;
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

        return services;
    }
}