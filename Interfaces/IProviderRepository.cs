using BookingSystem.Models;

namespace BookingSystem.Interfaces
{
    public interface IProviderRepository
    {
        // For Public Browsing
        Task<IEnumerable<Provider>> GetAllProvidersAsync();
        Task<Provider> GetProviderByIdAsync(int id);

        // For the Dashboard
        Task<Provider> GetProviderByUserIdAsync(string userId);
        Task<IEnumerable<Appointment>> GetProviderAppointmentsAsync(int providerId);
        Task<(int Today, int Week, int Month, decimal Revenue)> GetProviderStatsAsync(int providerId);

        // Actions
        Task<bool> UpdateAvailabilityAsync(int providerId, bool isAvailable);
        Task<bool> UpdateAppointmentStatusAsync(int appointmentId, string status);
    }
}