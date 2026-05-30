namespace CarServiceBookingSystem.Domain.Entities;

public class ServicePriceRule : BaseEntity
{
    public int ServiceId { get; set; }
    public Service Service { get; set; } = null!;

    public int? CarBrandId { get; set; }
    public CarBrand? CarBrand { get; set; }

    public int? CarModelId { get; set; }
    public CarModel? CarModel { get; set; }

    public int? CarYearId { get; set; }
    public CarYear? CarYear { get; set; }

    public int? CarTrimId { get; set; }
    public CarTrim? CarTrim { get; set; }

    public decimal Price { get; set; }

    public int DurationMinutes { get; set; }

    public bool IsActive { get; set; } = true;
}