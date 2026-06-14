using CarServiceBookingSystem.Application.Interfaces.IContext;
using CarServiceBookingSystem.Domain.Entities;
using CarServiceBookingSystem.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace CarServiceBookingSystem.Infrastructure.Persistence;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    private readonly ICurrentUserService _currentUserService;

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        ICurrentUserService currentUserService)
        : base(options)
    {
        _currentUserService = currentUserService;
    }

    public DbSet<CarBrand> CarBrands => Set<CarBrand>();
    public DbSet<CarModel> CarModels => Set<CarModel>();
    public DbSet<CarYear> CarYears => Set<CarYear>();
    public DbSet<CarTrim> CarTrims => Set<CarTrim>();
    public DbSet<Car> Cars => Set<Car>();
    public DbSet<Service> Services => Set<Service>();
    public DbSet<ServicePriceRule> ServicePriceRules => Set<ServicePriceRule>();
    public DbSet<Booking> Bookings => Set<Booking>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<SecurityAuditLog> SecurityAuditLogs => Set<SecurityAuditLog>();
    public DbSet<TrustedDevice> TrustedDevices => Set<TrustedDevice>();
    public DbSet<ApiKey> ApiKeys => Set<ApiKey>();
    public DbSet<IdempotencyKey> IdempotencyKeys { get; set; }
    public DbSet<StripeWebhookEvent> StripeWebhookEvents { get; set; }
    public DbSet<ServiceAreaRule> ServiceAreaRules => Set<ServiceAreaRule>();
    public DbSet<ServiceBranch> ServiceBranches => Set<ServiceBranch>();
    public DbSet<BranchWorkingHour> BranchWorkingHours => Set<BranchWorkingHour>();
    public DbSet<BranchService> BranchServices => Set<BranchService>();
    public DbSet<BranchClosure> BranchClosures => Set<BranchClosure>();
    public DbSet<BranchCapacityRule> BranchCapacityRules => Set<BranchCapacityRule>();
    public DbSet<Technician> Technicians { get; set; }
    public DbSet<TechnicianService> TechnicianServices { get; set; }
    public DbSet<TechnicianWorkingHour> TechnicianWorkingHours { get; set; }
    public DbSet<TechnicianUnavailableDate> TechnicianUnavailableDates { get; set; }
    public DbSet<CancellationPolicyRule> CancellationPolicyRules { get; set; }
    public DbSet<PromoCode> PromoCodes { get; set; }
    public DbSet<PromoCodeRedemption> PromoCodeRedemptions { get; set; }
    public DbSet<BookingReview> BookingReviews { get; set; }
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<NotificationPreference> NotificationPreferences => Set<NotificationPreference>();
    public DbSet<PushSubscription> PushSubscriptions => Set<PushSubscription>();
    public DbSet<AiConversation> AiConversations => Set<AiConversation>();
    public DbSet<AiConversationMessage> AiConversationMessages => Set<AiConversationMessage>();
    public DbSet<AiServiceRecommendation> AiServiceRecommendations => Set<AiServiceRecommendation>();
    public DbSet<AiAdvisorSetting> AiAdvisorSettings => Set<AiAdvisorSetting>();

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker.Entries<BaseIdEntity>();
        var userId = _currentUserService.UserId ?? "System";

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = DateTime.UtcNow;
                entry.Entity.IsDeleted = false;
                entry.Entity.CreatedBy = userId;
            }

            if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = DateTime.UtcNow;
                entry.Entity.UpdatedBy = userId;
            }

            if (entry.State == EntityState.Deleted)
            {
                entry.State = EntityState.Modified;
                entry.Entity.IsDeleted = true;
                entry.Entity.DeletedAt = DateTime.UtcNow;
                entry.Entity.DeletedBy = userId;
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<CarBrand>().HasQueryFilter(x => !x.IsDeleted);
        builder.Entity<CarModel>().HasQueryFilter(x => !x.IsDeleted);
        builder.Entity<CarYear>().HasQueryFilter(x => !x.IsDeleted);
        builder.Entity<CarTrim>().HasQueryFilter(x => !x.IsDeleted);
        builder.Entity<Car>().HasQueryFilter(x => !x.IsDeleted);
        builder.Entity<Service>().HasQueryFilter(x => !x.IsDeleted);
        builder.Entity<Booking>().HasQueryFilter(x => !x.IsDeleted);
        builder.Entity<Payment>().HasQueryFilter(x => !x.IsDeleted);
        builder.Entity<RefreshToken>().HasQueryFilter(x => !x.IsDeleted);
        builder.Entity<TrustedDevice>().HasQueryFilter(x => !x.IsDeleted);

        foreach (var entity in builder.Model.GetEntityTypes())
        {
            var tableName = entity.GetTableName();
            if (tableName == null) continue;

            string schema = "dbo"; // Fallback

            if (tableName.Contains("Booking") || tableName.Contains("CancellationPolicyRules"))
                schema = "booking";
            else if (tableName.Contains("Technician"))
                schema = "provider";
            else if (tableName.Contains("Payment") || tableName.Contains("Promo") || tableName.Contains("Stripe") || tableName.Contains("Idempotency"))
                schema = "finance";
            else if (tableName.Contains("Branch") || tableName.Contains("Car") || tableName.Contains("Service"))
                schema = "service";

            entity.SetSchema(schema);
        }

        foreach (var entity in builder.Model.GetEntityTypes())
        {
            var entityType = entity.ClrType;
            if (entityType.Namespace != null && entityType.Namespace.Contains("Microsoft.AspNetCore.Identity"))
            {
                entity.SetSchema("security");

                var currentTableName = entity.GetTableName();
                if (currentTableName != null && currentTableName.StartsWith("AspNet"))
                {
                    entity.SetTableName(currentTableName.Replace("AspNet", ""));
                }
            }
        }

        builder.Entity<ApplicationUser>().ToTable("User", "security");
        builder.Entity<RefreshToken>().ToTable(nameof(RefreshToken), "security");
        builder.Entity<TrustedDevice>().ToTable(nameof(TrustedDevice), "security");
        builder.Entity<SecurityAuditLog>().ToTable(nameof(SecurityAuditLog), "security");

        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

    }
}