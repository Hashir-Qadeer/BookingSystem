using BookingSystem.Models;

namespace BookingSystem.Interfaces
{
    public interface IBookingRepository
    {
        Task<IEnumerable<TimeSlot>> GetAvailableSlotsAsync(int providerId, DateTime date);
        Task<int> CreateAppointmentAsync(Appointment appointment);
        Task<Appointment> GetAppointmentDetailsAsync(int appointmentId);
    }
}
