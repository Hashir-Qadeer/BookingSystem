using BookingSystem.Models;

namespace BookingSystem.ViewModels
{
    public class ProvidersListViewModel
    {
        public List<Provider> Providers { get; set; } = new List<Provider>();
        public string SearchQuery { get; set; }
        public string SelectedCategory { get; set; }
        public string SortBy { get; set; } = "rating";
        public List<string> Categories { get; set; } = new List<string>();
        public int TotalProviders { get; set; }
    }

    public class ProviderDetailsViewModel
    {
        public Provider Provider { get; set; }
        public List<Service> Services { get; set; } = new List<Service>();
        public List<Review> Reviews { get; set; } = new List<Review>();
        public List<TimeSlot> AvailableSlots { get; set; } = new List<TimeSlot>();
        public decimal AverageRating { get; set; }
        public int TotalReviews { get; set; }
        public Dictionary<int, int> RatingDistribution { get; set; } = new Dictionary<int, int>();
    }

    public class ProviderDashboardViewModel
    {
        public Provider Provider { get; set; }
        public List<Appointment> TodayAppointments { get; set; } = new List<Appointment>();
        public List<Appointment> UpcomingAppointments { get; set; } = new List<Appointment>();
        public int TodayAppointmentsCount { get; set; }
        public int WeekAppointmentsCount { get; set; }
        public int MonthAppointmentsCount { get; set; }
        public decimal MonthRevenue { get; set; }
        public decimal AverageRating { get; set; }
        public int TotalReviews { get; set; }
    }

    public class ProviderScheduleViewModel
    {
        public Provider Provider { get; set; }
        //public List<ProviderSchedule> WeeklySchedule { get; set; } = new List<ProviderSchedule>();
        public List<TimeSlot> TimeSlots { get; set; } = new List<TimeSlot>();
        public DateTime SelectedDate { get; set; } = DateTime.Today;
    }

    public class ProviderAppointmentsViewModel
    {
        public Provider Provider { get; set; }
        public List<Appointment> TodayAppointments { get; set; } = new List<Appointment>();
        public List<Appointment> UpcomingAppointments { get; set; } = new List<Appointment>();
        public List<Appointment> PastAppointments { get; set; } = new List<Appointment>();
        public string FilterStatus { get; set; } = "all";
    }
}