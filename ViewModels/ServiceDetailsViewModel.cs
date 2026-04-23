using BookingSystem.Models;

namespace BookingSystem.ViewModels
{
    public class ServiceDetailsViewModel
    {
        public Service Service { get; set; }
        public List<Provider> AvailableProviders { get; set; } = new List<Provider>();
        public List<Review> Reviews { get; set; } = new List<Review>();
        public decimal AverageRating { get; set; }
        public int TotalReviews { get; set; }
        public Dictionary<int, int> RatingDistribution { get; set; } = new Dictionary<int, int>();
        public List<Service> RelatedServices { get; set; } = new List<Service>();
        public int SelectedProviderId { get; set; }
    }
}