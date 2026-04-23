using BookingSystem.Models;

namespace BookingSystem.ViewModels
{
    public class CustomerDashboardViewModels
    {
        public User Customer { get; set; }
        public List<Appointment> UpcomingAppointments { get; set; } = new List<Appointment>();
        public List<Appointment> PastAppointments { get; set; } = new List<Appointment>();
        public int TotalBookings { get; set; }
        public int CompletedBookings { get; set; }
        public int CancelledBookings { get; set; }
        public List<Service> FavoriteServices { get; set; } = new List<Service>();
    }
    public class MyAppointmentsViewModel
    {
        public List<Appointment> UpcomingAppointments { get; set; } = new List<Appointment>();
        public List<Appointment> PastAppointments { get; set; } = new List<Appointment>();
        public string FilterStatus { get; set; } = "all";
    }
}
