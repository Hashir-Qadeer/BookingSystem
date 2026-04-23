using BookingSystem.Models;

namespace BookingSystem.Interfaces
{
    public interface IAdminRepository
    {
        // Stats for Dashboard
        Task<int> GetTotalUsersCountAsync();
        Task<int> GetTotalProvidersCountAsync();
        Task<decimal> GetTotalRevenueAsync();
        Task<decimal> GetMonthRevenueAsync();

        // Management
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<IEnumerable<Provider>> GetAllProvidersAsync();
        Task<IEnumerable<Appointment>> GetAllAppointmentsAsync();

        // Actions
        Task<bool> ToggleServiceStatusAsync(int id);
        Task<bool> DeleteServiceAsync(int id);
        Task<bool> VerifyProviderAsync(int id);
        //asdghjkl
        Task<int> AddServiceAsync(Service service);
    }
}