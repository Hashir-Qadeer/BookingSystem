using BookingSystem.Models;

namespace BookingSystem.ViewModels
{
    public class HomeViewModel
    {
        public List<Service> FeaturedServices { get; set; } = new List<Service>();
        public List<Provider> TopProviders { get; set; } = new List<Provider>();
        public int TotalServices { get; set; }
        public int TotalProviders { get; set; }
        public int TotalBookings { get; set; }
        public List<string> ServiceCategories { get; set; } = new List<string>();
    }
}