namespace CarServiceBookingSystem.Domain.Entities
{
    public class ServiceBranch : BaseIdEntity
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
        public ICollection<BranchClosure> Closures { get; set; } = new List<BranchClosure>();
        public ICollection<BranchCapacityRule> CapacityRules { get; set; } = new List<BranchCapacityRule>();
        public ICollection<Technician> Technicians { get; set; } = new List<Technician>();
    }
}