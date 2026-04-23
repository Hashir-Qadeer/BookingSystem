using BookingSystem.Models;

namespace BookingSystem.ViewModels
{
    public class AdminDashboardViewModel
    {
        public int TotalUsers { get; set; }
        public int TotalProviders { get; set; }
        public int TotalCustomers { get; set; }
        public int TotalServices { get; set; }
        public int TotalAppointments { get; set; }
        public int TodayAppointments { get; set; }
        public int PendingAppointments { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal MonthRevenue { get; set; }
        public List<Appointment> RecentAppointments { get; set; } = new List<Appointment>();
        public List<Provider> TopProviders { get; set; } = new List<Provider>();
        public Dictionary<string, int> AppointmentsByStatus { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, decimal> RevenueByCategory { get; set; } = new Dictionary<string, decimal>();
    }

    public class AdminServicesViewModel
    {
        public List<Service> Services { get; set; } = new List<Service>();
        public List<ServiceCategory> Categories { get; set; } = new List<ServiceCategory>();
        public string SearchQuery { get; set; }
        public string SelectedCategory { get; set; }
        public int TotalServices { get; set; }
    }

    public class AdminProvidersViewModel
    {
        public List<Provider> Providers { get; set; } = new List<Provider>();
        public string SearchQuery { get; set; }
        public string FilterStatus { get; set; } = "all"; // all, verified, pending
        public int TotalProviders { get; set; }
        public int VerifiedProviders { get; set; }
        public int PendingProviders { get; set; }
    }

    public class AdminUsersViewModel
    {
        public List<User> Users { get; set; } = new List<User>();
        public string SearchQuery { get; set; }
        public string FilterRole { get; set; } = "all";
        public int TotalUsers { get; set; }
        public Dictionary<string, int> UsersByRole { get; set; } = new Dictionary<string, int>();
    }

    public class AdminAppointmentsViewModel
    {
        public List<Appointment> Appointments { get; set; } = new List<Appointment>();
        public string SearchQuery { get; set; }
        public string FilterStatus { get; set; } = "all";
        public DateTime? FilterDate { get; set; }
        public int TotalAppointments { get; set; }
        public Dictionary<string, int> AppointmentsByStatus { get; set; } = new Dictionary<string, int>();
    }

    public class ServiceCategory
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string Description { get; set; }
        public string IconClass { get; set; }
        public bool IsActive { get; set; }
    }
}