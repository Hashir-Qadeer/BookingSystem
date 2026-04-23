using BookingSystem.Models;

namespace BookingSystem.Interfaces
{
    public interface IServiceRepository
    {
        Task<IEnumerable<Service>> GetAllServicesAsync();
        Task<Service> GetServiceByIdAsync(int id);
        Task<IEnumerable<Service>> GetServicesByCategoryAsync(string categoryName);
        Task<IEnumerable<Provider>> GetProvidersByServiceIdAsync(int serviceId);
        Task<IEnumerable<Review>> GetReviewsByServiceIdAsync(int serviceId);
    }
}
