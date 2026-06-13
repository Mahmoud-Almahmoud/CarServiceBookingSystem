using CarServiceBookingSystem.Application.Common.Interfaces;
using CarServiceBookingSystem.Application.Interfaces;
using CarServiceBookingSystem.Application.Interfaces.IAuth;
using CarServiceBookingSystem.Application.Interfaces.IBackgrounJobs;
using CarServiceBookingSystem.Application.Interfaces.IBookings;
using CarServiceBookingSystem.Application.Interfaces.ICars;
using CarServiceBookingSystem.Application.Interfaces.IContext;
using CarServiceBookingSystem.Application.Interfaces.IEmail;
using CarServiceBookingSystem.Application.Interfaces.IGeoLocation;
using CarServiceBookingSystem.Application.Interfaces.INotification;
using CarServiceBookingSystem.Application.Interfaces.IPayments;
using CarServiceBookingSystem.Application.Interfaces.ISecurity;
using CarServiceBookingSystem.Application.Interfaces.IServices;
using CarServiceBookingSystem.Application.Interfaces.ITechnicians;
using CarServiceBookingSystem.Application.Interfaces.IUsers;
using CarServiceBookingSystem.Application.Interfaces.IUtils;
using CarServiceBookingSystem.Application.Options;
using CarServiceBookingSystem.Infrastructure.Context;
using CarServiceBookingSystem.Infrastructure.Identity;
using CarServiceBookingSystem.Infrastructure.Payments;
using CarServiceBookingSystem.Infrastructure.Persistence;
using CarServiceBookingSystem.Infrastructure.Services;
using CarServiceBookingSystem.Infrastructure.Services.Auth;
using CarServiceBookingSystem.Infrastructure.Services.BackgroundJobs;
using CarServiceBookingSystem.Infrastructure.Services.Bookings;
using CarServiceBookingSystem.Infrastructure.Services.Cars;
using CarServiceBookingSystem.Infrastructure.Services.CarServices;
using CarServiceBookingSystem.Infrastructure.Services.Email;
using CarServiceBookingSystem.Infrastructure.Services.GeoLocation;
using CarServiceBookingSystem.Infrastructure.Services.Notifications;
using CarServiceBookingSystem.Infrastructure.Services.Payments;
using CarServiceBookingSystem.Infrastructure.Services.Security;
using CarServiceBookingSystem.Infrastructure.Services.Technicians;
using CarServiceBookingSystem.Infrastructure.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Stripe;
using System.Text;

namespace CarServiceBookingSystem.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddIdentityCore<ApplicationUser>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequiredLength = 6;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireLowercase = true;
            options.Lockout.AllowedForNewUsers = true;
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
        })
        .AddRoles<IdentityRole>()
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();

        //services.Configure<JwtSettings>(configuration.GetSection("Jwt"));
        services.Configure<BookingQuoteOptions>(configuration.GetSection("BookingQuote"));
        services.Configure<BookingAvailabilityOptions>(configuration.GetSection("BookingAvailability"));
        //services.Configure<StripeSettings>(configuration.GetSection("Stripe"));
        //services.Configure<EmailSettings>(configuration.GetSection("Email"));
        services.Configure<BookingCleanupOptions>(configuration.GetSection("BookingCleanup"));
        //services.Configure<OpenRouteServiceOptions>(configuration.GetSection("OpenRouteService"));
        services.Configure<NotificationCleanupOptions>(configuration.GetSection("NotificationCleanup"));
        services.Configure<WebPushOptions>(configuration.GetSection("WebPush"));

        

        services.AddOptions<JwtOptions>()
            .Bind(configuration.GetSection("Jwt"))
            .Validate(x => !string.IsNullOrWhiteSpace(x.Key), "Jwt:Key is required.")
            .Validate(x => x.Key.Length >= 32, "Jwt:Key must be at least 32 characters.")
            .Validate(x => !string.IsNullOrWhiteSpace(x.Issuer), "Jwt:Issuer is required.")
            .Validate(x => !string.IsNullOrWhiteSpace(x.Audience), "Jwt:Audience is required.")
            .ValidateOnStart();

        services.AddOptions<EmailOptions>()
            .Bind(configuration.GetSection("Email"))
            .Validate(x => !string.IsNullOrWhiteSpace(x.SmtpServer), "Email:SmtpServer is required.")
            .Validate(x => x.Port > 0, "Email:Port must be greater than 0.")
            .Validate(x => !string.IsNullOrWhiteSpace(x.Username), "Email:Username is required.")
            .Validate(x => !string.IsNullOrWhiteSpace(x.Password), "Email:Password is required.")
            .ValidateOnStart();

        services.AddOptions<StripeOptions>()
            .Bind(configuration.GetSection("Stripe"))
            .Validate(x => !string.IsNullOrWhiteSpace(x.SecretKey), "Stripe:SecretKey is required.")
            .Validate(x => !string.IsNullOrWhiteSpace(x.WebhookSecret), "Stripe:WebhookSecret is required.")
            .Validate(x => !string.IsNullOrWhiteSpace(x.Currency), "Stripe:Currency is required.")
            .ValidateOnStart();

        services.AddOptions<OpenRouteServiceOptions>()
            .Bind(configuration.GetSection("OpenRouteService"))
            .Validate(x => !string.IsNullOrWhiteSpace(x.ApiKey), "OpenRouteService:ApiKey is required.")
            .ValidateOnStart();

        var jwtSettings = configuration.GetSection("Jwt").Get<JwtOptions>();
        var stripSettings = configuration.GetSection("Stripe").Get<StripeOptions>();

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                ValidIssuer = jwtSettings!.Issuer,
                ValidAudience = jwtSettings.Audience,

                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtSettings.Key))
            };
            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    var accessToken = context.Request.Query["access_token"];

                    var path = context.HttpContext.Request.Path;

                    if (!string.IsNullOrWhiteSpace(accessToken) &&
                        path.StartsWithSegments("/hubs/notifications"))
                    {
                        context.Token = accessToken;
                    }

                    return Task.CompletedTask;
                }
            };
        });

        //services.Configure<GoogleMapsOptions>(configuration.GetSection("GoogleMaps"));

        //services.AddHttpClient<IReverseGeocodingService, GoogleReverseGeocodingService>((serviceProvider, client) =>
        //{
        //    var options = serviceProvider
        //        .GetRequiredService<IOptions<GoogleMapsOptions>>()
        //        .Value;

        //    client.BaseAddress = new Uri(options.GeocodingBaseUrl);
        //});

        //services.AddHttpClient<ITravelEstimateService, GoogleRoutesTravelEstimateService>((serviceProvider, client) =>
        //{
        //    var options = serviceProvider
        //        .GetRequiredService<IOptions<GoogleMapsOptions>>()
        //        .Value;

        //    client.BaseAddress = new Uri(options.RoutesBaseUrl);
        //});


        services.AddHttpClient<IReverseGeocodingService, OpenRouteServiceReverseGeocodingService>((serviceProvider, client) =>
        {
            var options = serviceProvider
                .GetRequiredService<IOptions<OpenRouteServiceOptions>>()
                .Value;

            client.BaseAddress = new Uri(options.BaseUrl);
        });

        services.AddHttpClient<ITravelEstimateService, OpenRouteServiceTravelEstimateService>((serviceProvider, client) =>
        {
            var options = serviceProvider
                .GetRequiredService<IOptions<OpenRouteServiceOptions>>()
                .Value;

            client.BaseAddress = new Uri(options.BaseUrl);
        });

        StripeConfiguration.ApiKey = stripSettings?.SecretKey ?? configuration["StripeSettings:SecretKey"];

        services.AddMemoryCache();
        services.AddHttpContextAccessor();

        services.AddScoped<ITokenService, Services.Authentication.TokenService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<ICarService, CarService>();
        services.AddScoped<ICarLookupService, CarLookupService>();
        services.AddScoped<IServiceService, ServiceService>();
        services.AddScoped<IBookingService, BookingService>();
        services.AddScoped<IPaymentService, PaymentService>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IBackgroundJobService, HangfireBackgroundJobService>();
        services.AddScoped<ISecurityAuditService, SecurityAuditService>();
        services.AddScoped<ISecurityAuditQueryService, SecurityAuditQueryService>();
        services.AddScoped<IQrCodeService, QrCodeService>();
        services.AddScoped<IGeoLocationService, GeoLocationService>();
        services.AddScoped<IApiKeyService, ApiKeyService>();
        services.AddScoped<IIdempotencyService, IdempotencyService>();
        services.AddScoped<IIdempotencyContext, IdempotencyContext>();
        services.AddScoped<IIdempotencyCleanupService, IdempotencyCleanupService>();
        services.AddScoped<IRefreshTokenCleanupService, RefreshTokenCleanupService>();
        services.AddScoped<ISecurityAuditLogCleanupService, SecurityAuditLogCleanupService>();
        services.AddScoped<IStripeWebhookCleanupService, StripeWebhookCleanupService>();
        services.AddScoped<ITrustedDeviceCleanupService, TrustedDeviceCleanupService>();
        services.AddScoped<ISystemStatusService, SystemStatusService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<IPermissionService, PermissionService>();
        services.AddScoped<IBookingQuoteService, BookingQuoteService>();
        //services.AddScoped<ITravelEstimateService, MockTravelEstimateService>();
        services.AddScoped<IServicePricingService, ServicePricingService>();
        services.AddScoped<IBookingAvailabilityService, BookingAvailabilityService>();
        services.AddScoped<IServiceAreaService, ServiceAreaService>();
        services.AddScoped<PaymentIntentService>();
        services.AddScoped<IServiceBranchService, ServiceBranchService>();
        services.AddScoped<ITechnicianService, TechnicianManagementService>();
        services.AddScoped<IBookingAssignmentService, BookingAssignmentService>();
        services.AddScoped<IPaymentRefundService, PaymentRefundService>();
        services.AddScoped<ITechnicianScheduleService, TechnicianScheduleService>();
        services.AddScoped<IBookingCleanupJob, BookingCleanupJob>();
        services.AddScoped<ICancellationPolicyRuleService, CancellationPolicyRuleService>();
        services.AddScoped<IPromoCodeService, PromoCodeService>();
        services.AddScoped<IBookingReceiptService, BookingReceiptService>();
        services.AddScoped<IBookingReviewService, BookingReviewService>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<INotificationAudienceService, NotificationAudienceService>();
        services.AddScoped<INotificationCleanupJob, NotificationCleanupJob>();
        services.AddScoped<INotificationPreferenceService, NotificationPreferenceService>();
        services.AddScoped<IPushNotificationService, PushNotificationService>();

        return services;
    }
}