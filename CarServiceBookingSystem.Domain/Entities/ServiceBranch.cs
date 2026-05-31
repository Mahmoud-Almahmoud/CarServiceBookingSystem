namespace CarServiceBookingSystem.Domain.Entities
{
    public class ServiceBranch : BaseEntity
    {
        public string Name { get; set; } = string.Empty;

        public string CountryCode { get; set; } = string.Empty;

        public string City { get; set; } = string.Empty;

        public decimal Latitude { get; set; }

        public decimal Longitude { get; set; }

        public bool IsActive { get; set; } = true;

        public ICollection<BranchService> BranchServices { get; set; } = new List<BranchService>();

        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
        public ICollection<BranchWorkingHour> WorkingHours { get; set; } = new List<BranchWorkingHour>();
    }
}