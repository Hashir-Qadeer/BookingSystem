using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using BookingSystem.Models;
using System.Security.Claims;

namespace BookingSystem.Data
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

            // Ensure DB exists
            await context.Database.EnsureCreatedAsync();

            // 1. Seed Admin
            await CreateUserWithClaim(userManager, context, "admin@bookingsystem.com", "Admin@123", "Admin", "System", "Admin");

            // 2. Seed Provider Users
            string sarahId = await CreateUserWithClaim(userManager, context, "sarah.johnson@bookingsystem.com", "Provider@123", "Provider", "Sarah", "Johnson");
            string michaelId = await CreateUserWithClaim(userManager, context, "michael.chen@bookingsystem.com", "Provider@123", "Provider", "Michael", "Chen");

            // 3. Seed Providers Table
            if (sarahId != null && !await context.Providers.AnyAsync(p => p.UserId == sarahId))
            {
                context.Providers.Add(new Provider
                {
                    UserId = sarahId,
                    Specialization = "Hair Stylist",
                    Bio = "Expert stylist with 10+ years experience",
                    YearsOfExperience = 10,
                    IsAvailable = true,
                    IsVerified = true,
                    CreatedDate = DateTime.Now
                });
            }

            if (michaelId != null && !await context.Providers.AnyAsync(p => p.UserId == michaelId))
            {
                context.Providers.Add(new Provider
                {
                    UserId = michaelId,
                    Specialization = "General Physician",
                    Bio = "Board-certified family doctor",
                    YearsOfExperience = 15,
                    IsAvailable = true,
                    IsVerified = true,
                    CreatedDate = DateTime.Now
                });
            }

            // Must save here so we have Provider IDs for the next steps
            await context.SaveChangesAsync();

            // 4. Seed ProviderServices (The Junction)
            await SeedJunctionTable(context);

            // 5. Seed ProviderSchedules (The Working Hours)
            await SeedProviderSchedules(context);

            // 6. Seed Customers
            await CreateUserWithClaim(userManager, context, "john.doe@example.com", "Customer@123", "Customer", "John", "Doe");

            await context.SaveChangesAsync();
        }

        private static async Task SeedJunctionTable(ApplicationDbContext context)
        {
            if (await context.ProviderServices.AnyAsync()) return;

            var sarah = await context.Providers.FirstOrDefaultAsync(p => context.Users.Any(u => u.Id == p.UserId && u.Email == "sarah.johnson@bookingsystem.com"));
            var michael = await context.Providers.FirstOrDefaultAsync(p => context.Users.Any(u => u.Id == p.UserId && u.Email == "michael.chen@bookingsystem.com"));

            if (sarah != null)
            {
                context.ProviderServices.Add(new ProviderService { ProviderId = sarah.ProviderId, ServiceId = 1, IsActive = true, CreatedDate = DateTime.Now });
            }
            if (michael != null)
            {
                context.ProviderServices.Add(new ProviderService { ProviderId = michael.ProviderId, ServiceId = 2, IsActive = true, CreatedDate = DateTime.Now });
            }
            await context.SaveChangesAsync();
        }

        private static async Task SeedProviderSchedules(ApplicationDbContext context)
        {
            if (await context.ProviderSchedules.AnyAsync()) return;

            var providers = await context.Providers.ToListAsync();
            foreach (var prov in providers)
            {
                for (int day = 1; day <= 5; day++) // Monday to Friday
                {
                    context.ProviderSchedules.Add(new ProviderSchedule
                    {
                        ProviderId = prov.ProviderId,
                        DayOfWeek = day,
                        StartTime = new TimeSpan(9, 0, 0),
                        EndTime = new TimeSpan(17, 0, 0),
                        IsActive = true,
                        EffectiveFrom = DateTime.Today
                    });
                }
            }
            await context.SaveChangesAsync();
        }

        private static async Task<string> CreateUserWithClaim(UserManager<IdentityUser> userManager, ApplicationDbContext context, string email, string password, string type, string fName, string lName)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
            {
                user = new IdentityUser { UserName = email, Email = email, EmailConfirmed = true };
                var result = await userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    await userManager.AddClaimAsync(user, new Claim("UserType", type));
                    context.UserProfiles.Add(new UserProfile { UserId = user.Id, FirstName = fName, LastName = lName, CreatedDate = DateTime.Now });
                    await context.SaveChangesAsync();
                }
            }
            return user?.Id;
        }
    }
}