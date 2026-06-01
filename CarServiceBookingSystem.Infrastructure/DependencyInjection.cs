using CarServiceBookingSystem.Application.Common.Interfaces;
using CarServiceBookingSystem.Application.Interfaces;
using CarServiceBookingSystem.Application.Interfaces.IBackgrounJobs;
using CarServiceBookingSystem.Application.Options;
using CarServiceBookingSystem.Infrastructure.Authentication;
using CarServiceBookingSystem.Infrastructure.Identity;
using CarServiceBookingSystem.Infrastructure.Payments;
using CarServiceBookingSystem.Infrastructure.Persistence;
using CarServiceBookingSystem.Infrastructure.Services;
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

        services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
        services.Configure<BookingQuoteOptions>(configuration.GetSection("BookingQuote"));
        services.Configure<BookingAvailabilityOptions>(configuration.GetSection("BookingAvailability"));
        services.Configure<StripeSettings>(configuration.GetSection("StripeSettings"));
        services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));

        var jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettings>();
        var stripSettings = configuration.GetSection("StripeSettings").Get<StripeSettings>();

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
                    Encoding.UTF8.GetBytes(jwtSettings.Secret))
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

        services.Configure<OpenRouteServiceOptions>(configuration.GetSection("OpenRouteService"));

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

        services.AddScoped<ITokenService, Authentication.TokenService>();
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

        return services;
    }
}