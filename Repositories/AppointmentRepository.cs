using BookingSystem.Data;
using BookingSystem.Models;
using BookingSystem.Interfaces;
using Dapper;

namespace BookingSystem.Repositories
{
    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly DapperContext _context;
        public AppointmentRepository(DapperContext context) => _context = context;

        public async Task<IEnumerable<Appointment>> GetAppointmentsByCustomerIdAsync(string customerId)
        {
            var query = @"
                SELECT a.*, s.*, p.*, up.FirstName, up.LastName, up.ProfileImageUrl
                FROM Appointments a
                INNER JOIN Services s ON a.ServiceId = s.ServiceId
                INNER JOIN Providers p ON a.ProviderId = p.ProviderId
                INNER JOIN UserProfiles up ON p.UserId = up.UserId
                WHERE a.CustomerId = @CustomerId
                ORDER BY a.AppointmentDate DESC, a.StartTime DESC";

            using var connection = _context.CreateConnection();

            // Multi-mapping: (Appointment, Service, Provider) -> Appointment
            return await connection.QueryAsync<Appointment, Service, Provider, Appointment>(
                query, (appt, srv, prov) => {
                    appt.Service = srv;
                    appt.Provider = prov;
                    return appt;
                },
                new { CustomerId = customerId },
                splitOn: "ServiceId,ProviderId");
        }

        public async Task<User> GetCustomerProfileAsync(string userId)
        {
            var query = @"SELECT UserId, FirstName, LastName, ProfileImageUrl 
                          FROM UserProfiles WHERE UserId = @UserId";
            using var connection = _context.CreateConnection();
            return await connection.QuerySingleOrDefaultAsync<User>(query, new { UserId = userId });
        }

        public async Task<(int Total, int Completed, int Cancelled)> GetDashboardStatsAsync(string customerId)
        {
            // ISNULL ensures we get 0 instead of NULL if the user has no appointments
            var query = @"
        SELECT 
            COUNT(*) as Total,
            COALESCE(SUM(CASE WHEN Status = 'Completed' THEN 1 ELSE 0 END), 0) as Completed,
            COALESCE(SUM(CASE WHEN Status = 'Cancelled' THEN 1 ELSE 0 END), 0) as Cancelled
        FROM Appointments WHERE CustomerId = @CustomerId";

            using var connection = _context.CreateConnection();
            // Using dynamic to handle the row result safely
            var result = await connection.QuerySingleAsync(query, new { CustomerId = customerId });

            return ((int)result.Total, (int)result.Completed, (int)result.Cancelled);
        }

        public async Task<bool> CancelAppointmentAsync(int id)
        {
            var query = "UPDATE Appointments SET Status = 'Cancelled' WHERE AppointmentId = @Id";
            using var connection = _context.CreateConnection();
            return await connection.ExecuteAsync(query, new { Id = id }) > 0;
        }
        public async Task<IEnumerable<Appointment>> GetUpcomingAppointmentsAsync(string customerId)
        {
            var query = @"
        SELECT  a.*, s.*, p.*, up.FirstName, up.LastName
        FROM Appointments a
        INNER JOIN Services s ON a.ServiceId = s.ServiceId
        INNER JOIN Providers p ON a.ProviderId = p.ProviderId
        INNER JOIN UserProfiles up ON p.UserId = up.UserId
        WHERE a.CustomerId = @CustomerId AND a.AppointmentDate >= CURRENT_DATE AND a.Status = 'Confirmed'
        ORDER BY a.AppointmentDate ASC
        LIMIT 3";

            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<Appointment, Service, Provider, Appointment>(
                query, (appt, srv, prov) => {
                    appt.Service = srv;
                    appt.Provider = prov;
                    return appt;
                }, new { CustomerId = customerId }, splitOn: "ServiceId,ProviderId");
        }
    }
}