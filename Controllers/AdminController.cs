using BookingSystem.Interfaces;
using BookingSystem.Models;
using BookingSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly IAdminRepository _adminRepo;
    private readonly IServiceRepository _serviceRepo;

    public AdminController(IAdminRepository adminRepo, IServiceRepository serviceRepo)
    {
        _adminRepo = adminRepo;
        _serviceRepo = serviceRepo;
    }

    public async Task<IActionResult> Dashboard()
    {
        var appointments = await _adminRepo.GetAllAppointmentsAsync();
        var providers = await _adminRepo.GetAllProvidersAsync();

        var viewModel = new AdminDashboardViewModel
        {
            TotalUsers = await _adminRepo.GetTotalUsersCountAsync(),
            TotalProviders = await _adminRepo.GetTotalProvidersCountAsync(),
            TotalServices = (await _serviceRepo.GetAllServicesAsync()).Count(),
            TotalAppointments = appointments.Count(),
            TotalRevenue = await _adminRepo.GetTotalRevenueAsync(),
            MonthRevenue = await _adminRepo.GetMonthRevenueAsync(),
            RecentAppointments = appointments.Take(5).ToList(),
            TopProviders = providers.OrderByDescending(p => p.Rating).Take(3).ToList()
        };

        return View(viewModel);
    }

    public async Task<IActionResult> Services(string search = null, string category = null)
    {
        var services = await _serviceRepo.GetAllServicesAsync();

        if (!string.IsNullOrEmpty(search))
            services = services.Where(s => s.Name.Contains(search, StringComparison.OrdinalIgnoreCase));

        var viewModel = new AdminServicesViewModel
        {
            Services = services.ToList(),
            SearchQuery = search,
            SelectedCategory = category,
            TotalServices = services.Count()
        };

        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> ToggleServiceStatus(int id)
    {
        await _adminRepo.ToggleServiceStatusAsync(id);
        return Ok();
    }

    [HttpPost]
    public async Task<IActionResult> DeleteService(int id)
    {
        await _adminRepo.DeleteServiceAsync(id);
        return Ok();
    }
    // Add these missing Action Methods inside your AdminController class

    // GET: Admin/Users
    public async Task<IActionResult> Users(string search = null, string role = "all")
    {
        var users = await _adminRepo.GetAllUsersAsync();

        if (!string.IsNullOrEmpty(search))
        {
            users = users.Where(u =>
                (u.FirstName + " " + u.LastName).Contains(search, StringComparison.OrdinalIgnoreCase) ||
                u.Email.Contains(search, StringComparison.OrdinalIgnoreCase));
        }

        if (role != "all")
        {
            users = users.Where(u => u.Role == role);
        }

        var viewModel = new AdminUsersViewModel
        {
            Users = users.ToList(),
            SearchQuery = search,
            FilterRole = role,
            TotalUsers = users.Count()
        };

        return View(viewModel);
    }

    // GET: Admin/Appointments
    public async Task<IActionResult> Appointments(string search = null, string status = "all")
    {
        var appointments = await _adminRepo.GetAllAppointmentsAsync();

        if (!string.IsNullOrEmpty(search))
        {
            appointments = appointments.Where(a =>
                a.Service.Name.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                a.CustomerName.Contains(search, StringComparison.OrdinalIgnoreCase));
        }

        if (status != "all")
        {
            appointments = appointments.Where(a => a.Status == status);
        }

        var viewModel = new AdminAppointmentsViewModel
        {
            Appointments = appointments.ToList(),
            SearchQuery = search,
            FilterStatus = status,
            TotalAppointments = appointments.Count()
        };

        return View(viewModel);
    }

    // POST: Admin/DeactivateUser
    [HttpPost]
    public async Task<IActionResult> DeactivateUser(string id)
    {
        // Implementation for deactivation logic in Repository
        // await _adminRepo.DeactivateUserAsync(id); 
        return Ok();
    }

    // POST: Admin/AddService (Fixes the Modal issue)
    [HttpPost]
    public async Task<IActionResult> AddService(Service service)
    {
        if (ModelState.IsValid)
        {
            service.IsActive = true;
            // You'll need to add AddServiceAsync to your IAdminRepository and implementation
            // await _adminRepo.AddServiceAsync(service); 
            return RedirectToAction("Services");
        }
        return BadRequest();
    }
}