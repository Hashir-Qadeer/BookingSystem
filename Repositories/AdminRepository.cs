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
            var sql = "SELECT COUNT(*) FROM AspNetUsers";
            using var connection = _context.CreateConnection();
            return await connection.ExecuteScalarAsync<int>(sql);
        }

        public async Task<decimal> GetTotalRevenueAsync()
        {
            var sql = "SELECT ISNULL(SUM(TotalPrice), 0) FROM Appointments WHERE Status = 'Completed'";
            using var connection = _context.CreateConnection();
            return await connection.ExecuteScalarAsync<decimal>(sql);
        }

        public async Task<IEnumerable<Appointment>> GetAllAppointmentsAsync()
        {
            var sql = @"SELECT a.*, s.*, p.*, up.FirstName + ' ' + up.LastName as CustomerName
                        FROM Appointments a
                        INNER JOIN Services s ON a.ServiceId = s.ServiceId
                        INNER JOIN Providers p ON a.ProviderId = p.ProviderId
                        INNER JOIN UserProfiles up ON a.CustomerId = up.UserId
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
            var sql = "UPDATE Services SET IsActive = CASE WHEN IsActive = 1 THEN 0 ELSE 1 END WHERE ServiceId = @Id";
            using var connection = _context.CreateConnection();
            return await connection.ExecuteAsync(sql, new { Id = id }) > 0;
        }

        public async Task<bool> DeleteServiceAsync(int id)
        {
            var sql = "DELETE FROM Services WHERE ServiceId = @Id";
            using var connection = _context.CreateConnection();
            return await connection.ExecuteAsync(sql, new { Id = id }) > 0;
        }

        public async Task<int> GetTotalProvidersCountAsync()
        {
            using var connection = _context.CreateConnection();
            return await connection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM Providers");
        }

        public async Task<decimal> GetMonthRevenueAsync()
        {
            using var connection = _context.CreateConnection();
            return await connection.ExecuteScalarAsync<decimal>(
                "SELECT ISNULL(SUM(TotalPrice), 0) FROM Appointments WHERE Status = 'Completed' AND MONTH(AppointmentDate) = MONTH(GETDATE())");
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<User>(
                "SELECT up.*, u.Email FROM UserProfiles up JOIN AspNetUsers u ON up.UserId = u.Id");
        }

        public async Task<IEnumerable<Provider>> GetAllProvidersAsync()
        {
            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<Provider>(
                "SELECT p.*, up.FirstName, up.LastName FROM Providers p JOIN UserProfiles up ON p.UserId = up.UserId");
        }

        public async Task<bool> VerifyProviderAsync(int id)
        {
            using var connection = _context.CreateConnection();
            return await connection.ExecuteAsync("UPDATE Providers SET IsVerified = 1 WHERE ProviderId = @Id", new { Id = id }) > 0;
        }

        // Fix for Error CS0535: Implementation of AddServiceAsync
        public async Task<int> AddServiceAsync(Service service)
        {
            var sql = @"INSERT INTO Services (CategoryId, Name, Description, Price, DurationMinutes, Category, ImageUrl, IsActive, CreatedDate)
                        VALUES (@CategoryId, @Name, @Description, @Price, @DurationMinutes, @Category, @ImageUrl, @IsActive, @CreatedDate);
                        SELECT CAST(SCOPE_IDENTITY() as int);";

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
    } // End of Class
} // End of Namespace