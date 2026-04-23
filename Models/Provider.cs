using Microsoft.AspNetCore.Identity;

namespace BookingSystem.Models
{
    public class Provider
    {
        public int ProviderId { get; set; }
        public string UserId { get; set; }
        public string Specialization { get; set; }
        public string Bio { get; set; }
        public int YearsOfExperience { get; set; }
        public bool IsAvailable { get; set; }
        public bool IsVerified { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }

        // Navigation property
        public IdentityUser User { get; set; }

        // Display properties (not mapped to DB)
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName => $"{FirstName} {LastName}";
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string ProfileImageUrl { get; set; }
        public decimal Rating { get; set; }
        public int TotalReviews { get; set; }
        public List<int> ServiceIds { get; set; } = new List<int>();
    }
    //public class Provider
    //{
    //    public int ProviderId { get; set; }
    //    public string UserId { get; set; }
    //    public IdentityUser User { get; set; }
    //    public string Specialization { get; set; }
    //    public string Bio { get; set; }
    //    public int YearsOfExperience { get; set; }
    //    public bool IsAvailable { get; set; }
    //    public bool IsVerified { get; set; }
    //    public DateTime CreatedDate { get; set; }
    //}

}
