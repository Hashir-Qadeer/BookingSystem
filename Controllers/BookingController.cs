using Microsoft.AspNetCore.Mvc;
using BookingSystem.Models;
using BookingSystem.ViewModels;
using BookingSystem.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using BookingSystem.Interfaces;

namespace BookingSystem.Controllers
{
    [Authorize] // Users must be logged in to book
    public class BookingController : Controller
    {
        private readonly IServiceRepository _serviceRepo;
        private readonly IBookingRepository _bookingRepo;
        private readonly UserManager<IdentityUser> _userManager;

        public BookingController(IServiceRepository serviceRepo, IBookingRepository bookingRepo, UserManager<IdentityUser> userManager)
        {
            _serviceRepo = serviceRepo;
            _bookingRepo = bookingRepo;
            _userManager = userManager;
        }

        public async Task<IActionResult> Book(int serviceId, int providerId, DateTime? date)
        {
            var service = await _serviceRepo.GetServiceByIdAsync(serviceId);
            var providers = await _serviceRepo.GetProvidersByServiceIdAsync(serviceId);
            var provider = providers.FirstOrDefault(p => p.ProviderId == providerId);

            if (service == null || provider == null) return RedirectToAction("Index", "Services");

            var selectedDate = date ?? DateTime.Today.AddDays(1);
            var slots = await _bookingRepo.GetAvailableSlotsAsync(providerId, selectedDate);

            var viewModel = new BookingViewModel
            {
                Service = service,
                Provider = provider,
                SelectedDate = selectedDate,
                AvailableSlots = slots.ToList()
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> GetTimeSlots(int providerId, DateTime date)
        {
            var slots = await _bookingRepo.GetAvailableSlotsAsync(providerId, date);
            return Json(slots);
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmBooking(int serviceId, int providerId, string startTime, string selectedDate, string notes)
        {
            var service = await _serviceRepo.GetServiceByIdAsync(serviceId);
            if (service == null) return BadRequest("Service not found");

            var userId = _userManager.GetUserId(User);

            // Use TryParse to prevent the FormatException crash
            if (!TimeSpan.TryParse(startTime, out TimeSpan start))
            {
                return BadRequest("Invalid time format selected.");
            }

            if (!DateTime.TryParse(selectedDate, out DateTime appointmentDate))
            {
                appointmentDate = DateTime.Today.AddDays(1);
            }

            var appointment = new Appointment
            {
                CustomerId = userId,
                ProviderId = providerId,
                ServiceId = serviceId,
                AppointmentDate = appointmentDate,
                StartTime = start,
                EndTime = start.Add(TimeSpan.FromMinutes(service.DurationMinutes)),
                Status = "Confirmed",
                CustomerNotes = notes,
                TotalPrice = service.Price,
                CreatedDate = DateTime.Now
            };

            int appointmentId = await _bookingRepo.CreateAppointmentAsync(appointment);
            return RedirectToAction("Confirmation", new { id = appointmentId });
        }
        public async Task<IActionResult> Confirmation(int id)
        {
            var appointment = await _bookingRepo.GetAppointmentDetailsAsync(id);
            if (appointment == null) return NotFound();

            var viewModel = new BookingConfirmationViewModel
            {
                Appointment = appointment,
                Service = appointment.Service,
                Provider = appointment.Provider,
                ConfirmationNumber = $"BK-{id:D6}"
            };

            return View(viewModel);
        }
    }
}