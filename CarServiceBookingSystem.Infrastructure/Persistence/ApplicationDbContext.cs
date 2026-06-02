using CarServiceBookingSystem.Application.Interfaces;
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

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker.Entries<BaseEntity>();
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

        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        builder.Entity<ServiceAreaRule>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.CountryCode)
                .IsRequired()
                .HasMaxLength(10);

            entity.Property(x => x.City)
                .HasMaxLength(100);

            entity.Property(x => x.IsAllowed)
                .IsRequired();

            entity.Property(x => x.Priority)
                .IsRequired();

            entity.Property(x => x.IsActive)
                .IsRequired();

            entity.HasOne(x => x.Service)
                .WithMany()
                .HasForeignKey(x => x.ServiceId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(x => new
            {
                x.ServiceId,
                x.CountryCode,
                x.City,
                x.IsActive
            });

            entity.HasIndex(x => x.Priority);
        });
        builder.Entity<StripeWebhookEvent>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.StripeEventId)
                .HasMaxLength(200)
                .IsRequired();

            entity.Property(x => x.EventType)
                .HasMaxLength(200)
                .IsRequired();

            entity.HasIndex(x => x.StripeEventId)
                .IsUnique();
        });

        builder.Entity<ApiKey>()
            .HasIndex(x => x.KeyHash)
            .IsUnique();

        builder.Entity<CarBrand>()
            .HasMany(x => x.Models)
            .WithOne(x => x.Brand)
            .HasForeignKey(x => x.BrandId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<CarModel>()
            .HasMany(x => x.Years)
            .WithOne(x => x.Model)
            .HasForeignKey(x => x.ModelId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<CarYear>()
            .HasMany(x => x.Trims)
            .WithOne(x => x.Year)
            .HasForeignKey(x => x.YearId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<CarTrim>()
            .HasMany(x => x.Cars)
            .WithOne(x => x.CarTrim)
            .HasForeignKey(x => x.CarTrimId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Car>()
            .HasMany<Booking>()
            .WithOne(x => x.Car)
            .HasForeignKey(x => x.CarId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Service>()
            .HasMany<Booking>()
            .WithOne(x => x.Service)
            .HasForeignKey(x => x.ServiceId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Booking>()
            .HasOne(x => x.Payment)
           .WithOne(x => x.Booking)
           .HasForeignKey<Payment>(x => x.BookingId)
           .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Booking>(entity =>
        {
            entity.Property(x => x.ServicePrice)
                .HasPrecision(18, 2);

            entity.Property(x => x.TravelFee)
                .HasPrecision(18, 2);

            entity.Property(x => x.TotalPrice)
                .HasPrecision(18, 2);

            entity.Property(x => x.CustomerLatitude)
                .HasPrecision(10, 7);

            entity.Property(x => x.CustomerLongitude)
                .HasPrecision(10, 7);

            entity.Property(x => x.CustomerCountryCode)
                .HasMaxLength(10);

            entity.Property(x => x.CustomerCity)
                .HasMaxLength(100);

            entity.Property(x => x.DistanceKm)
                .HasPrecision(10, 2);

            entity.HasOne(x => x.ServiceBranch)
                .WithMany(x => x.Bookings)
                .HasForeignKey(x => x.ServiceBranchId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.ServiceBranch)
                .WithMany(x => x.Bookings)
                .HasForeignKey(x => x.ServiceBranchId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.Property(x => x.CancellationReason)
                .HasMaxLength(500);

            entity.Property(x => x.CancelledByUserId)
                .HasMaxLength(450);
        });
           


        builder.Entity<ServicePriceRule>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Price)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            entity.Property(x => x.DurationMinutes)
                .IsRequired();

            entity.Property(x => x.IsActive)
                .IsRequired();

            entity.HasOne(x => x.Service)
                .WithMany(x => x.PriceRules)
                .HasForeignKey(x => x.ServiceId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.CarBrand)
                .WithMany()
                .HasForeignKey(x => x.CarBrandId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.CarModel)
                .WithMany()
                .HasForeignKey(x => x.CarModelId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.CarTrim)
                .WithMany()
                .HasForeignKey(x => x.CarTrimId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(x => x.ServiceId);

            entity.HasIndex(x => new
            {
                x.ServiceId,
                x.CarBrandId,
                x.CarModelId,
                x.CarTrimId,
                x.CarYearId,
                x.IsActive
            });
        });

        builder.Entity<Service>()
            .Property(x => x.Price)
            .HasColumnType("decimal(18,2)");

        builder.Entity<Payment>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Amount)
                .HasPrecision(18, 2);

            entity.Property(x => x.Currency)
                .IsRequired()
                .HasMaxLength(10);

            entity.Property(x => x.PaymentIntentId)
                .HasMaxLength(200);

            entity.Property(x => x.StripeClientSecret)
                .HasMaxLength(500);

            entity.Property(x => x.FailureReason)
                .HasMaxLength(1000);

            entity.HasOne(x => x.Booking)
                .WithOne(x => x.Payment)
                .HasForeignKey<Payment>(x => x.BookingId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(x => x.BookingId)
                 .IsUnique();

            entity.HasIndex(x => x.PaymentIntentId)
                .IsUnique()
                .HasFilter("[PaymentIntentId] IS NOT NULL");
        });

        builder.Entity<ServiceBranch>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(150);

            entity.Property(x => x.CountryCode)
                .IsRequired()
                .HasMaxLength(10);

            entity.Property(x => x.City)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(x => x.Latitude)
                .HasPrecision(10, 7);

            entity.Property(x => x.Longitude)
                .HasPrecision(10, 7);

            entity.Property(x => x.IsActive)
                .IsRequired();

            entity.HasIndex(x => x.Name);

            entity.HasIndex(x => new
            {
                x.CountryCode,
                x.City,
                x.IsActive
            });
        });

        builder.Entity<BranchService>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.IsActive)
                .IsRequired();

            entity.HasOne(x => x.ServiceBranch)
                .WithMany(x => x.BranchServices)
                .HasForeignKey(x => x.ServiceBranchId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.Service)
                .WithMany()
                .HasForeignKey(x => x.ServiceId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(x => new
            {
                x.ServiceBranchId,
                x.ServiceId
            }).IsUnique();
        });

        builder.Entity<BranchWorkingHour>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.DayOfWeek)
                .IsRequired();

            entity.Property(x => x.OpenTime)
                .IsRequired();

            entity.Property(x => x.CloseTime)
                .IsRequired();

            entity.Property(x => x.IsClosed)
                .IsRequired();

            entity.HasOne(x => x.ServiceBranch)
                .WithMany(x => x.WorkingHours)
                .HasForeignKey(x => x.ServiceBranchId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(x => new
            {
                x.ServiceBranchId,
                x.DayOfWeek
            }).IsUnique();
        });

        builder.Entity<BranchClosure>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.StartDate)
                .IsRequired();

            entity.Property(x => x.EndDate)
                .IsRequired();

            entity.Property(x => x.IsFullDay)
                .IsRequired();

            entity.Property(x => x.Type)
                .IsRequired();

            entity.Property(x => x.Reason)
                .IsRequired()
                .HasMaxLength(300);

            entity.Property(x => x.IsActive)
                .IsRequired();

            entity.HasOne(x => x.ServiceBranch)
                .WithMany(x => x.Closures)
                .HasForeignKey(x => x.ServiceBranchId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(x => new
            {
                x.ServiceBranchId,
                x.StartDate,
                x.EndDate,
                x.IsActive
            });
        });

        builder.Entity<BranchCapacityRule>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Capacity)
                .IsRequired();

            entity.Property(x => x.IsActive)
                .IsRequired();

            entity.HasOne(x => x.ServiceBranch)
                .WithMany(x => x.CapacityRules)
                .HasForeignKey(x => x.ServiceBranchId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(x => new
            {
                x.ServiceBranchId,
                x.DayOfWeek,
                x.StartTime,
                x.EndTime,
                x.IsActive
            });
        });
    }
}