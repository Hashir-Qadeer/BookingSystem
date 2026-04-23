using BookingSystem.Interfaces;
using BookingSystem.Models;
using BookingSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

[Authorize]
public class ProviderController : Controller
{
    private readonly IProviderRepository _providerRepo;
    private readonly IServiceRepository _serviceRepo;
    private readonly UserManager<IdentityUser> _userManager;

    public ProviderController(IProviderRepository providerRepo, IServiceRepository serviceRepo, UserManager<IdentityUser> userManager)
    {
        _providerRepo = providerRepo;
        _serviceRepo = serviceRepo;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index(string search = null, string category = null, string sortBy = "rating")
    {
        var providers = await _providerRepo.GetAllProvidersAsync();
        // ... (Keep your filtering/sorting logic here, using 'providers' from DB)

        var viewModel = new ProvidersListViewModel
        {
            Providers = providers.ToList(),
            TotalProviders = providers.Count(),
            Categories = providers.Select(p => p.Specialization).Distinct().ToList()
        };
        return View(viewModel);
    }

    public async Task<IActionResult> Dashboard()
    {
        var userId = _userManager.GetUserId(User);
        var provider = await _providerRepo.GetProviderByUserIdAsync(userId);
        if (provider == null) return RedirectToAction("Index", "Home");

        var appointments = await _providerRepo.GetProviderAppointmentsAsync(provider.ProviderId);
        var stats = await _providerRepo.GetProviderStatsAsync(provider.ProviderId);

        var viewModel = new ProviderDashboardViewModel
        {
            Provider = provider,
            TodayAppointments = appointments.Where(a => a.AppointmentDate == DateTime.Today).ToList(),
            UpcomingAppointments = appointments.Where(a => a.AppointmentDate > DateTime.Today).ToList(),
            TodayAppointmentsCount = stats.Today,
            WeekAppointmentsCount = stats.Week,
            MonthAppointmentsCount = stats.Month,
            MonthRevenue = stats.Revenue,
            AverageRating = provider.Rating,
            TotalReviews = provider.TotalReviews
        };
        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateAppointmentStatus(int id, string status)
    {
        await _providerRepo.UpdateAppointmentStatusAsync(id, status);
        return Ok();
    }

    [HttpPost]
    public async Task<IActionResult> UpdateAvailability(bool isAvailable)
    {
        var userId = _userManager.GetUserId(User);
        var provider = await _providerRepo.GetProviderByUserIdAsync(userId);
        await _providerRepo.UpdateAvailabilityAsync(provider.ProviderId, isAvailable);
        return RedirectToAction("Dashboard");
    }
    // Add this method to handle the Details page
    public async Task<IActionResult> Details(int id)
    {
        var provider = await _providerRepo.GetProviderByIdAsync(id);
        if (provider == null) return NotFound();

        // Get services offered by this provider
        var allServices = await _serviceRepo.GetAllServicesAsync();
        var providerServices = allServices.Where(s =>
            _providerRepo.GetProviderAppointmentsAsync(id).Result.Any(a => a.ServiceId == s.ServiceId)).ToList();

        var viewModel = new ProviderDetailsViewModel
        {
            Provider = provider,
            Services = providerServices,
            AverageRating = provider.Rating,
            TotalReviews = provider.TotalReviews,
            RatingDistribution = new Dictionary<int, int> { { 5, 10 }, { 4, 2 }, { 3, 1 }, { 2, 0 }, { 1, 0 } } // Dummy for now
        };

        return View(viewModel);
    }

    // Add this method to handle the MyAppointments page
    public async Task<IActionResult> MyAppointments(string status = "all")
    {
        var userId = _userManager.GetUserId(User);
        var provider = await _providerRepo.GetProviderByUserIdAsync(userId);
        if (provider == null) return RedirectToAction("Index", "Home");

        // 1. Get all appointments from the repository
        var appointments = await _providerRepo.GetProviderAppointmentsAsync(provider.ProviderId);

        // 2. Apply status filter
        // Note: Use status.ToLower() to prevent case-sensitivity issues
        if (status.ToLower() != "all")
        {
            appointments = appointments.Where(a => a.Status.Equals(status, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        // 3. Map to ViewModel
        var viewModel = new ProviderAppointmentsViewModel
        {
            Provider = provider,
            // We filter the already status-filtered list into time categories for the View
            TodayAppointments = appointments.Where(a => a.AppointmentDate.Date == DateTime.Today).ToList(),
            UpcomingAppointments = appointments.Where(a => a.AppointmentDate.Date > DateTime.Today).ToList(),
            PastAppointments = appointments.Where(a => a.AppointmentDate.Date < DateTime.Today).ToList(),
            FilterStatus = status
        };

        return View(viewModel);
    }
    // Add this method to handle the Schedule page
    public async Task<IActionResult> Schedule()
    {
        var userId = _userManager.GetUserId(User);
        var provider = await _providerRepo.GetProviderByUserIdAsync(userId);
        if (provider == null) return RedirectToAction("Index", "Home");

        var viewModel = new ProviderScheduleViewModel
        {
            Provider = provider,
            SelectedDate = DateTime.Today
        };

        return View(viewModel);
    }
}
