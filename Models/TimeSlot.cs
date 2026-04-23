namespace BookingSystem.Models
{
    public class TimeSlot
    {
        public int TimeSlotId { get; set; }
        public int ProviderId { get; set; }
        public int? ServiceId { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public bool IsAvailable { get; set; }
        public bool IsBooked { get; set; }
        public int? AppointmentId { get; set; }

        // For display purposes
        public string DisplayTime => $"{StartTime:hh\\:mm} - {EndTime:hh\\:mm}";
    }

}