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
            int dayOfWeek = (int)date.DayOfWeek;
            var scheduleQuery = @"SELECT * FROM providerschedules 
                                  WHERE ProviderId = @ProviderId AND DayOfWeek = @Day 
                                  AND IsActive = true";
            var bookedQuery = @"SELECT StartTime, EndTime FROM appointments 
                                WHERE ProviderId = @ProviderId AND AppointmentDate = @Date 
                                AND Status != 'Cancelled'";
            using var connection = _context.CreateConnection();
            var schedule = await connection.QueryFirstOrDefaultAsync<ProviderSchedule>(scheduleQuery, new { ProviderId = providerId, Day = dayOfWeek });
            var bookedSlots = await connection.QueryAsync<Appointment>(bookedQuery, new { ProviderId = providerId, Date = date });
            var availableSlots = new List<TimeSlot>();
            if (schedule == null) return availableSlots;
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
            var sql = @"INSERT INTO appointments (CustomerId, ProviderId, ServiceId, AppointmentDate, StartTime, EndTime, Status, CustomerNotes, TotalPrice, CreatedDate)
                        VALUES (@CustomerId, @ProviderId, @ServiceId, @AppointmentDate, @StartTime, @EndTime, @Status, @CustomerNotes, @TotalPrice, @CreatedDate)
                        RETURNING ""AppointmentId""";
            using var connection = _context.CreateConnection();
            return await connection.QuerySingleAsync<int>(sql, appt);
        }

        public async Task<Appointment> GetAppointmentDetailsAsync(int id)
        {
            var sql = @"SELECT a.*, s.*, p.*, up.FirstName, up.LastName, up.ProfileImageUrl
                        FROM appointments a
                        INNER JOIN services s ON a.ServiceId = s.ServiceId
                        INNER JOIN providers p ON a.ProviderId = p.ProviderId
                        INNER JOIN userprofiles up ON p.UserId = up.UserId
                        WHERE a.AppointmentId = @Id";
            using var connection = _context.CreateConnection();
            var result = await connection.QueryAsync<Appointment, Service, Provider, Appointment>(sql,
                (appt, srv, prov) => {
                    appt.Service = srv;
                    appt.Provider = prov;
                    return appt;
                }, new { Id = id }, splitOn: "ServiceId,ProviderId");
            return result.FirstOrDefault();
        }
    }
}