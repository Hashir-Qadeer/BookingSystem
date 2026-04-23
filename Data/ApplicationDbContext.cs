using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using BookingSystem.Models;
using BookingSystem.ViewModels;

namespace BookingSystem.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<Models.ServiceCategory> ServiceCategories { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Provider> Providers { get; set; }
        public DbSet<ProviderService> ProviderServices { get; set; }
        public DbSet<ProviderSchedule> ProviderSchedules { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<TimeSlot> TimeSlots { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // 1. Primary Key Mappings (Fixes the InvalidOperationExceptions)
            builder.Entity<UserProfile>().HasKey(up => up.UserProfileId);
            builder.Entity<Models.ServiceCategory>().HasKey(sc => sc.CategoryId);
            builder.Entity<Service>().HasKey(s => s.ServiceId);
            builder.Entity<Provider>().HasKey(p => p.ProviderId);
            builder.Entity<ProviderService>().HasKey(ps => ps.ProviderServiceId);
            builder.Entity<ProviderSchedule>().HasKey(ps => ps.ScheduleId);
            builder.Entity<Appointment>().HasKey(a => a.AppointmentId);
            builder.Entity<TimeSlot>().HasKey(ts => ts.TimeSlotId);
            builder.Entity<Payment>().HasKey(p => p.PaymentId);
            builder.Entity<Review>().HasKey(r => r.ReviewId);
            builder.Entity<Notification>().HasKey(n => n.NotificationId);

            // 2. Ignore "Display Only" properties (Prevents EF from trying to create columns for them)
            // These properties exist in your classes but aren't in your SQL tables
            builder.Entity<Provider>().Ignore(p => p.FirstName);
            builder.Entity<Provider>().Ignore(p => p.LastName);
            builder.Entity<Provider>().Ignore(p => p.Email);
            builder.Entity<Provider>().Ignore(p => p.PhoneNumber);
            builder.Entity<Provider>().Ignore(p => p.ProfileImageUrl);
            builder.Entity<Provider>().Ignore(p => p.Rating);
            builder.Entity<Provider>().Ignore(p => p.TotalReviews);
            builder.Entity<Provider>().Ignore(p => p.ServiceIds);

            builder.Entity<Service>().Ignore(s => s.Category);

            builder.Entity<Review>().Ignore(r => r.CustomerName);
            builder.Entity<Review>().Ignore(r => r.CustomerImageUrl);

            // 3. Unique Constraints (Optional but good practice)
            builder.Entity<UserProfile>().HasIndex(up => up.UserId).IsUnique();
            builder.Entity<Provider>().HasIndex(p => p.UserId).IsUnique();
        }
    }
}