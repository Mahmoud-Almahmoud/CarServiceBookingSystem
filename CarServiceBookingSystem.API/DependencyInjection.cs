using Asp.Versioning;
using CarServiceBookingSystem.API.Auth;
using CarServiceBookingSystem.API.Filters;
using CarServiceBookingSystem.API.Services;
using CarServiceBookingSystem.API.SignalR;
using CarServiceBookingSystem.Application;
using CarServiceBookingSystem.Application.DTOs.Auth;
using CarServiceBookingSystem.Application.DTOs.Bookings;
using CarServiceBookingSystem.Application.DTOs.Cars;
using CarServiceBookingSystem.Application.DTOs.Services;
using CarServiceBookingSystem.Application.Interfaces.INotification;
using CarServiceBookingSystem.Infrastructure;
using FluentValidation;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace CarServiceBookingSystem.API
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddAPIDependencies(this IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
        {
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());


            services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;

                options.ApiVersionReader = new UrlSegmentApiVersionReader();
            })
            .AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });

            services.AddScoped<ValidationFilter<RegisterRequest>>();
            services.AddScoped<ValidationFilter<LoginRequest>>();
            services.AddScoped<ValidationFilter<CreateCarRequest>>();
            services.AddScoped<ValidationFilter<UpdateCarRequest>>();
            services.AddScoped<ValidationFilter<CreateBookingRequest>>();
            services.AddScoped<ValidationFilter<UpdateBookingStatusRequest>>();
            services.AddScoped<ValidationFilter<CreateServiceRequest>>();
            services.AddScoped<ValidationFilter<UpdateServiceRequest>>();
            services.AddScoped<ValidationFilter<ResendEmailConfirmationRequest>>();
            services.AddScoped<ValidationFilter<ForgotPasswordRequest>>();
            services.AddScoped<ValidationFilter<ResetPasswordRequest>>();
            services.AddScoped<ValidationFilter<ChangePasswordRequest>>();
            services.AddScoped<ValidationFilter<VerifyTwoFactorRequest>>();
            services.AddScoped<ValidationFilter<LoginTwoFactorRequest>>();
            services.AddScoped<ValidationFilter<DisableTwoFactorRequest>>();
            services.AddScoped<ValidationFilter<LoginRecoveryCodeRequest>>();
            services.AddScoped<ValidationFilter<BookingQuoteRequest>>();
            services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
            services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();

            services.AddControllers()
                    .AddJsonOptions(options =>
                    {
                        options.JsonSerializerOptions.Converters.Add(
                            new JsonStringEnumConverter());
                    });

            services.AddApplication();
            if (environment.IsEnvironment("Testing"))
            {
                configuration["Jwt:Key"] =
                    "THIS_IS_A_TEST_SECRET_KEY_FOR_INTEGRATION_TESTS_123456789";

                configuration["Jwt:Issuer"] =
                    "CarServiceBookingSystem";

                configuration["Jwt:Audience"] =
                    "CarServiceBookingSystemUsers";

                configuration["Jwt:AccessTokenExpirationMinutes"] =
                    "15";

                configuration["Jwt:RefreshTokenExpirationDays"] =
                    "30";
            }
            services.AddInfrastructure(configuration);

            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Car Service Booking System API",
                    Version = "v1"
                });

                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter JWT token only"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        []
                    }
                });
                        });

            if (!environment.IsEnvironment("Testing"))
            {
                services.AddHangfire(config =>
                {
                    config.UseSqlServerStorage(
                        configuration.GetConnectionString("DefaultConnection"));
                });

                services.AddHangfireServer();
            }

            // SignalR
            services.AddSignalR();
            services.AddSingleton<IUserIdProvider, CustomUserIdProvider>();
            services.AddScoped<INotificationRealtimeService, NotificationRealtimeService>();


            return services;
        }
    }
}
