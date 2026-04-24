using BookingSystem.Data;
using BookingSystem.Interfaces;
using BookingSystem.Models;
using Dapper;
namespace BookingSystem.Repositories
{
    public class BookingRepository : IBookingRepository
    {
        private readonly DapperContext _context;

        public BookingRepository(DapperContext context) => _context = context;

        public async Task<IEnumerable<TimeSlot>> GetAvailableSlotsAsync(int providerId, DateTime date)
        {
            // 1. Get Provider's standard schedule for that day of week
            int dayOfWeek = (int)date.DayOfWeek;
            var scheduleQuery = @"SELECT * FROM ProviderSchedules 
                                 WHERE ProviderId = @ProviderId AND DayOfWeek = @Day 
                                 AND IsActive = true";

            // 2. Get already booked appointments for that day
            var bookedQuery = @"SELECT StartTime, EndTime FROM Appointments 
                               WHERE ProviderId = @ProviderId AND AppointmentDate = @Date 
                               AND Status != 'Cancelled'";

            using var connection = _context.CreateConnection();
            var schedule = await connection.QueryFirstOrDefaultAsync<ProviderSchedule>(scheduleQuery, new { ProviderId = providerId, Day = dayOfWeek });
            var bookedSlots = await connection.QueryAsync<Appointment>(bookedQuery, new { ProviderId = providerId, Date = date });

            var availableSlots = new List<TimeSlot>();
            if (schedule == null) return availableSlots;

            // 3. Logic to generate 30-minute slots between StartTime and EndTime
            var current = schedule.StartTime;
            while (current < schedule.EndTime)
            {
                var next = current.Add(TimeSpan.FromMinutes(30));
                bool isBooked = bookedSlots.Any(b => b.StartTime == current);

                availableSlots.Add(new TimeSlot
                {
                    ProviderId = providerId,
                    Date = date,
                    StartTime = current,
                    EndTime = next,
                    IsAvailable = !isBooked,
                    IsBooked = isBooked
                });
                current = next;
            }

            return availableSlots;
        }

        public async Task<int> CreateAppointmentAsync(Appointment appt)
        {
            var sql = @"INSERT INTO Appointments (CustomerId, ProviderId, ServiceId, AppointmentDate, StartTime, EndTime, Status, CustomerNotes, TotalPrice, CreatedDate)
                        VALUES (@CustomerId, @ProviderId, @ServiceId, @AppointmentDate, @StartTime, @EndTime, @Status, @CustomerNotes, @TotalPrice, @CreatedDate)
                        RETURNING ""AppointmentId""";

            using var connection = _context.CreateConnection();
            return await connection.QuerySingleAsync<int>(sql, appt);
        }

        public async Task<Appointment> GetAppointmentDetailsAsync(int id)
        {
            // Ensure we select up.ProfileImageUrl for the provider and s.ImageUrl for the service
            var sql = @"SELECT a.*, s.*, p.*, up.FirstName, up.LastName, up.ProfileImageUrl
                FROM Appointments a
                INNER JOIN Services s ON a.ServiceId = s.ServiceId
                INNER JOIN Providers p ON a.ProviderId = p.ProviderId
                INNER JOIN UserProfiles up ON p.UserId = up.UserId
                WHERE a.AppointmentId = @Id";

            using var connection = _context.CreateConnection();
            var result = await connection.QueryAsync<Appointment, Service, Provider, Appointment>(sql,
                (appt, srv, prov) =>
                {
                    appt.Service = srv;
                    appt.Provider = prov;
                    return appt;
                }, new { Id = id }, splitOn: "ServiceId,ProviderId");

            return result.FirstOrDefault();
        }
    }
}
