using BookingSystem.Models;

namespace BookingSystem.Interfaces
{
    public interface IAppointmentRepository
    {
        Task<IEnumerable<Appointment>> GetAppointmentsByCustomerIdAsync(string customerId);
        Task<User> GetCustomerProfileAsync(string userId);
        Task<bool> CancelAppointmentAsync(int appointmentId);
        // Helper to get stats for the dashboard bubbles
        Task<(int Total, int Completed, int Cancelled)> GetDashboardStatsAsync(string customerId);
        Task<IEnumerable<Appointment>> GetUpcomingAppointmentsAsync(string customerId);

    }
}
