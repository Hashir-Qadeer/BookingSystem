using BookingSystem.Models;

namespace BookingSystem.ViewModels
{
    public class ServicesViewModel
    {
        public List<Service> Services { get; set; } = new List<Service>();
        public List<string> Categories { get; set; } = new List<string>();
        public string SelectedCategory { get; set; }
        public string SearchQuery { get; set; }
        public string SortBy { get; set; } = "name"; // name, price-low, price-high, duration
        public decimal MinPrice { get; set; } = 0;
        public decimal MaxPrice { get; set; } = 200;
        public int TotalServices { get; set; }
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 9;
        public int TotalPages { get; set; }
    }
}