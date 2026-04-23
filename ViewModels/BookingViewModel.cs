using BookingSystem.Models;

namespace BookingSystem.ViewModels
{
    public class BookingViewModel
    {
        public Service Service { get; set; }
        public Provider Provider { get; set; }
        public DateTime SelectedDate { get; set; } = DateTime.Today;
        public List<TimeSlot> AvailableSlots { get; set; } = new List<TimeSlot>();
        public string CustomerNotes { get; set; }
        public int? SelectedTimeSlotId { get; set; }
    }

    public class BookingConfirmationViewModel
    {
        public Appointment Appointment { get; set; }
        public Service Service { get; set; }
        public Provider Provider { get; set; }
        public string ConfirmationNumber { get; set; }
    }
}