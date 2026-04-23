namespace BookingSystem.Models
{
    public class User
    {
        public string UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName => $"{FirstName} {LastName}";
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string ProfileImageUrl { get; set; }
        public string Role { get; set; } // Customer, Provider, Admin
        public DateTime RegisteredDate { get; set; }
        public bool IsActive { get; set; }
    }
}