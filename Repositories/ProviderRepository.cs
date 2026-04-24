using BookingSystem.Data;
using BookingSystem.Models;
using BookingSystem.Interfaces;
using Dapper;

namespace BookingSystem.Repositories
{
    public class ProviderRepository : IProviderRepository
    {
        private readonly DapperContext _context;
        public ProviderRepository(DapperContext context) => _context = context;

        public async Task<IEnumerable<Provider>> GetAllProvidersAsync()
        {
            var query = @"SELECT p.*, up.FirstName, up.LastName, up.ProfileImageUrl 
                          FROM Providers p 
                          INNER JOIN UserProfiles up ON p.UserId = up.UserId";
            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<Provider>(query);
        }

        public async Task<Provider> GetProviderByIdAsync(int id)
        {
            var query = @"SELECT p.*, up.FirstName, up.LastName, up.ProfileImageUrl 
                          FROM Providers p 
                          INNER JOIN UserProfiles up ON p.UserId = up.UserId 
                          WHERE p.ProviderId = @Id";
            using var connection = _context.CreateConnection();
            return await connection.QuerySingleOrDefaultAsync<Provider>(query, new { Id = id });
        }

        public async Task<Provider> GetProviderByUserIdAsync(string userId)
        {
            var query = @"SELECT p.*, up.FirstName, up.LastName, up.ProfileImageUrl 
                          FROM Providers p 
                          INNER JOIN UserProfiles up ON p.UserId = up.UserId 
                          WHERE p.UserId = @UserId";
            using var connection = _context.CreateConnection();
            return await connection.QuerySingleOrDefaultAsync<Provider>(query, new { UserId = userId });
        }

        public async Task<IEnumerable<Appointment>> GetProviderAppointmentsAsync(int providerId)
        {
            // We join Appointments (a) with Services (s) and UserProfiles (up) 
            // to get the Customer's real name.
            var query = @"SELECT a.*, s.*, 
                  up.FirstName || ' ' || up.LastName as CustomerName
                  FROM Appointments a
                  INNER JOIN Services s ON a.ServiceId = s.ServiceId
                  INNER JOIN UserProfiles up ON a.CustomerId = up.UserId
                  WHERE a.ProviderId = @Id
                  ORDER BY a.AppointmentDate ASC, a.StartTime ASC";

            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<Appointment, Service, Appointment>(
                query,
                (appt, srv) => {
                    appt.Service = srv;
                    // The 'CustomerName' string from the SQL is automatically 
                    // mapped to your appt.CustomerName property by Dapper.
                    return appt;
                },
                new { Id = providerId },
                splitOn: "ServiceId");
        }

        public async Task<(int Today, int Week, int Month, decimal Revenue)> GetProviderStatsAsync(int providerId)
        {
            var query = @"
                SELECT 
                    COUNT(CASE WHEN AppointmentDate = CURRENT_DATE THEN 1 END) as Today,
                    COUNT(CASE WHEN AppointmentDate BETWEEN CURRENT_DATE AND CURRENT_DATE + INTERVAL '7 days' THEN 1 END) as Week,
                    COUNT(CASE WHEN EXTRACT(MONTH FROM AppointmentDate) = EXTRACT(MONTH FROM NOW()) AND EXTRACT(YEAR FROM AppointmentDate) = EXTRACT(YEAR FROM NOW()) THEN 1 END) as Month,
                    COALESCE(SUM(CASE WHEN EXTRACT(MONTH FROM AppointmentDate) = EXTRACT(MONTH FROM NOW()) AND Status = 'Completed' THEN TotalPrice ELSE 0 END), 0) as Revenue
                FROM Appointments WHERE ProviderId = @Id";
            using var connection = _context.CreateConnection();
            var res = await connection.QuerySingleAsync(query, new { Id = providerId });
            return ((int)res.Today, (int)res.Week, (int)res.Month, (decimal)res.Revenue);
        }

        public async Task<bool> UpdateAvailabilityAsync(int providerId, bool isAvailable)
        {
            var query = "UPDATE Providers SET IsAvailable = @Status WHERE ProviderId = @Id";
            using var connection = _context.CreateConnection();
            return await connection.ExecuteAsync(query, new { Status = isAvailable, Id = providerId }) > 0;
        }

        public async Task<bool> UpdateAppointmentStatusAsync(int appointmentId, string status)
        {
            var query = "UPDATE Appointments SET Status = @Status WHERE AppointmentId = @Id";
            using var connection = _context.CreateConnection();
            return await connection.ExecuteAsync(query, new { Status = status, Id = appointmentId }) > 0;
        }
    }
}
