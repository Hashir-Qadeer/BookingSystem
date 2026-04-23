using Microsoft.AspNetCore.Mvc;
using BookingSystem.ViewModels;
using BookingSystem.Interfaces;

namespace BookingSystem.Controllers
{
    public class ServicesController : Controller
    {
        private readonly IServiceRepository _serviceRepository;

        public ServicesController(IServiceRepository serviceRepository)
        {
            _serviceRepository = serviceRepository;
        }

        // ============================
        // SERVICES LIST PAGE
        // ============================
        public async Task<IActionResult> Index(
            string category = null,
            string search = null,
            string sortBy = "name",
            decimal minPrice = 0,
            decimal maxPrice = 200)
        {
            var allServices = (await _serviceRepository.GetAllServicesAsync()).ToList();

            var filteredServices = allServices
                .Where(s => s.IsActive)
                .ToList();

            if (!string.IsNullOrEmpty(category))
            {
                filteredServices = filteredServices
                    .Where(s => s.Category.Equals(category, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            if (!string.IsNullOrEmpty(search))
            {
                filteredServices = filteredServices
                    .Where(s =>
                        s.Name.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                        s.Description.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                        s.Category.Contains(search, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            filteredServices = filteredServices
                .Where(s => s.Price >= minPrice && s.Price <= maxPrice)
                .ToList();

            filteredServices = sortBy switch
            {
                "price-low" => filteredServices.OrderBy(s => s.Price).ToList(),
                "price-high" => filteredServices.OrderByDescending(s => s.Price).ToList(),
                "duration" => filteredServices.OrderBy(s => s.DurationMinutes).ToList(),
                _ => filteredServices.OrderBy(s => s.Name).ToList()
            };

            var viewModel = new ServicesViewModel
            {
                Services = filteredServices,
                Categories = allServices.Select(s => s.Category).Distinct().OrderBy(c => c).ToList(),
                SelectedCategory = category,
                SearchQuery = search,
                SortBy = sortBy,
                MinPrice = minPrice,
                MaxPrice = maxPrice,
                TotalServices = filteredServices.Count
            };

            return View(viewModel);
        }

        // ============================
        // SERVICE DETAILS PAGE
        // ============================
        public async Task<IActionResult> Details(int id)
        {
            var service = await _serviceRepository.GetServiceByIdAsync(id);
            if (service == null)
                return NotFound();

            var providers = (await _serviceRepository.GetProvidersByServiceIdAsync(id)).ToList();
            var reviews = (await _serviceRepository.GetReviewsByServiceIdAsync(id)).ToList();

            var ratingDistribution = new Dictionary<int, int>
            {
                { 5, reviews.Count(r => r.Rating == 5) },
                { 4, reviews.Count(r => r.Rating == 4) },
                { 3, reviews.Count(r => r.Rating == 3) },
                { 2, reviews.Count(r => r.Rating == 2) },
                { 1, reviews.Count(r => r.Rating == 1) }
            };

            var relatedServices = (await _serviceRepository
                .GetServicesByCategoryAsync(service.Category))
                .Where(s => s.ServiceId != id && s.IsActive)
                .Take(3)
                .ToList();

            var viewModel = new ServiceDetailsViewModel
            {
                Service = service,
                AvailableProviders = providers,
                Reviews = reviews,
                AverageRating = reviews.Any() ? (decimal)reviews.Average(r => r.Rating) : 0,
                TotalReviews = reviews.Count,
                RatingDistribution = ratingDistribution,
                RelatedServices = relatedServices
            };

            return View(viewModel);
        }
    }
}
