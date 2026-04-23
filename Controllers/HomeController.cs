using Microsoft.AspNetCore.Mvc;
using BookingSystem.Models;
using BookingSystem.ViewModels;
using BookingSystem.Interfaces;

namespace BookingSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly IServiceRepository _serviceRepo;
        private readonly IProviderRepository _providerRepo;
        private readonly IAdminRepository _adminRepo;

        public HomeController(
            IServiceRepository serviceRepo,
            IProviderRepository providerRepo,
            IAdminRepository adminRepo)
        {
            _serviceRepo = serviceRepo;
            _providerRepo = providerRepo;
            _adminRepo = adminRepo;
        }

        public async Task<IActionResult> Index()
        {
            // Fetch real data from Repositories
            var allServices = await _serviceRepo.GetAllServicesAsync();
            var allProviders = await _providerRepo.GetAllProvidersAsync();

            // Fetch system-wide stats from Admin Repo
            int totalUsers = await _adminRepo.GetTotalUsersCountAsync();
            int totalProvidersCount = await _adminRepo.GetTotalProvidersCountAsync();

            // Create a custom query or use GetAllAppointments count for bookings
            var appointments = await _adminRepo.GetAllAppointmentsAsync();

            var viewModel = new HomeViewModel
            {
                // Show only active services on the home page
                FeaturedServices = allServices.Where(s => s.IsActive).Take(6).ToList(),

                // Show providers with highest ratings
                TopProviders = allProviders.OrderByDescending(p => p.Rating).Take(4).ToList(),

                // Real Stats
                TotalServices = allServices.Count(),
                TotalProviders = totalProvidersCount,
                TotalBookings = appointments.Count(),

                ServiceCategories = allServices.Select(s => s.Category).Distinct().ToList()
            };

            return View(viewModel);
        }

        public IActionResult About() => View();
        public IActionResult Contact() => View();
        public IActionResult Privacy() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() => View();
    }
}