namespace CarServiceBookingSystem.Domain.Entities
{
    public class BranchWorkingHour : BaseIdEntity
    {
        public int ServiceBranchId { get; set; }

        public ServiceBranch ServiceBranch { get; set; } = null!;

        public DayOfWeek DayOfWeek { get; set; }

        public TimeSpan OpenTime { get; set; }

        public TimeSpan CloseTime { get; set; }

        public bool IsClosed { get; set; }
    }
}