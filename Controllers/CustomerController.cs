using Microsoft.AspNetCore.Mvc;
using BookingSystem.Models;
using BookingSystem.ViewModels;
using BookingSystem.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace BookingSystem.Controllers
{
    [Authorize] // Ensure only logged in customers access this
    public class CustomerController : Controller
    {
        private readonly IAppointmentRepository _appointmentRepo;
        private readonly UserManager<IdentityUser> _userManager;

        public CustomerController(IAppointmentRepository appointmentRepo, UserManager<IdentityUser> userManager)
        {
            _appointmentRepo = appointmentRepo;
            _userManager = userManager;
        }

        public async Task<IActionResult> Dashboard()
        {
            var userId = _userManager.GetUserId(User);
            var appointments = (await _appointmentRepo.GetAppointmentsByCustomerIdAsync(userId)).ToList();
            var customer = await _appointmentRepo.GetCustomerProfileAsync(userId);
            var stats = await _appointmentRepo.GetDashboardStatsAsync(userId);

            var viewModel = new CustomerDashboardViewModels
            {
                Customer = customer,
                UpcomingAppointments = appointments.Where(a => a.AppointmentDate >= DateTime.Today && a.Status != "Cancelled")
                                                   .OrderBy(a => a.AppointmentDate).Take(3).ToList(),
                PastAppointments = appointments.Where(a => a.Status == "Completed" || a.AppointmentDate < DateTime.Today)
                                               .OrderByDescending(a => a.AppointmentDate).Take(3).ToList(),
                TotalBookings = stats.Total,
                CompletedBookings = stats.Completed,
                CancelledBookings = stats.Cancelled
            };

            return View(viewModel);
        }

        public async Task<IActionResult> MyAppointments(string status = "all")
        {
            var userId = _userManager.GetUserId(User);
            var appointments = await _appointmentRepo.GetAppointmentsByCustomerIdAsync(userId);

            // Normalize status for logic
            var filtered = status.ToLower() switch
            {
                "upcoming" => appointments.Where(a => a.AppointmentDate >= DateTime.Today && a.Status != "Cancelled"),
                "completed" => appointments.Where(a => a.Status == "Completed"),
                "cancelled" => appointments.Where(a => a.Status == "Cancelled"),
                _ => appointments
            };

            var viewModel = new MyAppointmentsViewModel
            {
                // Upcoming = Future date AND not cancelled
                UpcomingAppointments = filtered.Where(a => a.AppointmentDate >= DateTime.Today && a.Status != "Cancelled").ToList(),

                // Past = Date is in the past OR status is Completed OR status is Cancelled
                PastAppointments = filtered.Where(a => a.AppointmentDate < DateTime.Today || a.Status == "Completed" || a.Status == "Cancelled").ToList(),

                FilterStatus = status
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> CancelAppointment(int id)
        {
            await _appointmentRepo.CancelAppointmentAsync(id);
            TempData["SuccessMessage"] = "Appointment cancelled successfully.";
            return RedirectToAction("MyAppointments");
        }
    }
}