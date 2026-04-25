using BookingSystem.Data;
using BookingSystem.Models;
using BookingSystem.Interfaces;
using Dapper;

namespace BookingSystem.Repositories
{
    public class AdminRepository : IAdminRepository
    {
        private readonly DapperContext _context;
        public AdminRepository(DapperContext context) => _context = context;

        public async Task<int> GetTotalUsersCountAsync()
        {
            var sql = "SELECT COUNT(*) FROM \"AspNetUsers\"";
            using var connection = _context.CreateConnection();
            return await connection.ExecuteScalarAsync<int>(sql);
        }

        public async Task<decimal> GetTotalRevenueAsync()
        {
            var sql = "SELECT COALESCE(SUM(TotalPrice), 0) FROM appointments WHERE Status = 'Completed'";
            using var connection = _context.CreateConnection();
            return await connection.ExecuteScalarAsync<decimal>(sql);
        }

        public async Task<IEnumerable<Appointment>> GetAllAppointmentsAsync()
        {
            var sql = @"SELECT a.*, s.*, p.*, up.FirstName || ' ' || up.LastName as CustomerName
                        FROM appointments a
                        INNER JOIN services s ON a.ServiceId = s.ServiceId
                        INNER JOIN providers p ON a.ProviderId = p.ProviderId
                        INNER JOIN userprofiles up ON a.CustomerId = up.UserId
                        ORDER BY a.CreatedDate DESC";
            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<Appointment, Service, Provider, Appointment>(sql, (appt, srv, prov) => {
                appt.Service = srv;
                appt.Provider = prov;
                return appt;
            }, splitOn: "ServiceId,ProviderId");
        }

        public async Task<bool> ToggleServiceStatusAsync(int id)
        {
            var sql = "UPDATE services SET IsActive = NOT IsActive WHERE ServiceId = @Id";
            using var connection = _context.CreateConnection();
            return await connection.ExecuteAsync(sql, new { Id = id }) > 0;
        }

        public async Task<bool> DeleteServiceAsync(int id)
        {
            var sql = "DELETE FROM services WHERE ServiceId = @Id";
            using var connection = _context.CreateConnection();
            return await connection.ExecuteAsync(sql, new { Id = id }) > 0;
        }

        public async Task<int> GetTotalProvidersCountAsync()
        {
            using var connection = _context.CreateConnection();
            return await connection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM providers");
        }

        public async Task<decimal> GetMonthRevenueAsync()
        {
            using var connection = _context.CreateConnection();
            return await connection.ExecuteScalarAsync<decimal>(
                "SELECT COALESCE(SUM(TotalPrice), 0) FROM appointments WHERE Status = 'Completed' AND EXTRACT(MONTH FROM AppointmentDate) = EXTRACT(MONTH FROM NOW())");
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<User>(
                "SELECT up.*, u.Email FROM userprofiles up JOIN \"AspNetUsers\" u ON up.UserId = u.Id");
        }

        public async Task<IEnumerable<Provider>> GetAllProvidersAsync()
        {
            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<Provider>(
                "SELECT p.*, up.FirstName, up.LastName FROM providers p JOIN userprofiles up ON p.UserId = up.UserId");
        }

        public async Task<bool> VerifyProviderAsync(int id)
        {
            using var connection = _context.CreateConnection();
            return await connection.ExecuteAsync("UPDATE providers SET IsVerified = true WHERE ProviderId = @Id", new { Id = id }) > 0;
        }

        public async Task<int> AddServiceAsync(Service service)
        {
            var sql = @"INSERT INTO services (CategoryId, Name, Description, Price, DurationMinutes, Category, ImageUrl, IsActive, CreatedDate)
                        VALUES (@CategoryId, @Name, @Description, @Price, @DurationMinutes, @Category, @ImageUrl, @IsActive, @CreatedDate)
                        RETURNING ""ServiceId""";
            using var connection = _context.CreateConnection();
            return await connection.QuerySingleAsync<int>(sql, new
            {
                service.CategoryId,
                service.Name,
                service.Description,
                service.Price,
                service.DurationMinutes,
                Category = service.Category ?? "General",
                service.ImageUrl,
                IsActive = true,
                CreatedDate = DateTime.Now
            });
        }
    }
}