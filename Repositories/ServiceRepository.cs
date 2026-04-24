using BookingSystem.Data;
using BookingSystem.Interfaces;
using BookingSystem.Models;
using Dapper;
namespace BookingSystem.Repositories
{
    public class ServiceRepository : IServiceRepository
    {
        private readonly DapperContext _context;

        public ServiceRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Service>> GetAllServicesAsync()
        {
            var query = @"SELECT s.*, c.CategoryName as Category 
                          FROM Services s 
                          INNER JOIN ServiceCategories c ON s.CategoryId = c.CategoryId 
                          WHERE s.IsActive = true";

            using (var connection = _context.CreateConnection())
            {
                return await connection.QueryAsync<Service>(query);
            }
        }

        public async Task<Service> GetServiceByIdAsync(int id)
        {
            var query = @"SELECT s.*, c.CategoryName as Category 
                          FROM Services s 
                          INNER JOIN ServiceCategories c ON s.CategoryId = c.CategoryId 
                          WHERE s.ServiceId = @Id";

            using (var connection = _context.CreateConnection())
            {
                return await connection.QuerySingleOrDefaultAsync<Service>(query, new { Id = id });
            }
        }

        public async Task<IEnumerable<Provider>> GetProvidersByServiceIdAsync(int serviceId)
        {
            // Join Providers with UserProfiles to get Names and Images
            var query = @"SELECT p.*, up.FirstName, up.LastName, up.ProfileImageUrl
                          FROM Providers p
                          INNER JOIN UserProfiles up ON p.UserId = up.UserId
                          INNER JOIN ProviderServices ps ON p.ProviderId = ps.ProviderId
                          WHERE ps.ServiceId = @ServiceId AND ps.IsActive = true";

            using (var connection = _context.CreateConnection())
            {
                return await connection.QueryAsync<Provider>(query, new { ServiceId = serviceId });
            }
        }

        public async Task<IEnumerable<Review>> GetReviewsByServiceIdAsync(int serviceId)
        {
            var query = @"SELECT r.*, up.FirstName || ' ' || up.LastName as CustomerName, up.ProfileImageUrl as CustomerImageUrl
                          FROM Reviews r
                          INNER JOIN UserProfiles up ON r.CustomerId = up.UserId
                          WHERE r.ServiceId = @ServiceId
                          ORDER BY r.CreatedDate DESC";

            using (var connection = _context.CreateConnection())
            {
                return await connection.QueryAsync<Review>(query, new { ServiceId = serviceId });
            }
        }

        public async Task<IEnumerable<Service>> GetServicesByCategoryAsync(string categoryName)
        {
            var query = @"SELECT s.*, c.CategoryName as Category 
                          FROM Services s 
                          INNER JOIN ServiceCategories c ON s.CategoryId = c.CategoryId 
                          WHERE c.CategoryName = @CatName AND s.IsActive = true";

            using (var connection = _context.CreateConnection())
            {
                return await connection.QueryAsync<Service>(query, new { CatName = categoryName });
            }
        }
    }
}
