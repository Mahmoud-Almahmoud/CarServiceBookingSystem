using CarServiceBookingSystem.Application.Interfaces;
using CarServiceBookingSystem.Domain.Entities;
using CarServiceBookingSystem.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

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
    public DbSet<Booking> Bookings => Set<Booking>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

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

        builder.Entity<Service>()
            .Property(x => x.Price)
            .HasColumnType("decimal(18,2)");

        builder.Entity<Payment>()
            .Property(x => x.Amount)
            .HasColumnType("decimal(18,2)");
    }
}