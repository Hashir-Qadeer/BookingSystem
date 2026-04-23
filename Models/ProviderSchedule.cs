namespace BookingSystem.Models
{
    public class ProviderSchedule
    {
        public int ScheduleId { get; set; }
        public int ProviderId { get; set; }
        public int DayOfWeek { get; set; } // 0=Sunday, 1=Monday, etc.
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public bool IsActive { get; set; }
        public DateTime EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
    }
}
