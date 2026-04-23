namespace BookingSystem.Models
{
    public class Appointment
    {
        public int AppointmentId { get; set; }
        public string CustomerId { get; set; }
        public int ProviderId { get; set; }
        public int ServiceId { get; set; }
        public DateTime AppointmentDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string Status { get; set; } // Pending, Confirmed, Completed, Cancelled
        public string CustomerNotes { get; set; }
        public string ProviderNotes { get; set; }
        public decimal TotalPrice { get; set; } // ADDED THIS
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }

        // Navigation properties (for display)
        public Service Service { get; set; }
        public Provider Provider { get; set; }
       //asdfghjkl
        public string? CustomerName { get; set; }
    }
}
//public class Appointment
//{
//    public int AppointmentId { get; set; }
//    public string CustomerId { get; set; }
//    public int ProviderId { get; set; }
//    public int ServiceId { get; set; }
//    public DateTime AppointmentDate { get; set; }
//    public TimeSpan StartTime { get; set; }
//    public TimeSpan EndTime { get; set; }
//    public string Status { get; set; }
//    public string CustomerNotes { get; set; }
//    public decimal TotalPrice { get; set; }
//    public DateTime CreatedDate { get; set; }
//}