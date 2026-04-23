namespace BookingSystem.Models
{
    public class Review
    {
        public int ReviewId { get; set; }
        public string CustomerId { get; set; }
        public int ServiceId { get; set; }
        public int ProviderId { get; set; }
        public int? AppointmentId { get; set; }
        public int Rating { get; set; } // 1-5
        public string Comment { get; set; }
        public bool IsVerified { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }

        // Display properties (not mapped)
        public string CustomerName { get; set; }
        public string CustomerImageUrl { get; set; }
    }

    //public class Review
    //{
    //    public int ReviewId { get; set; }
    //    public string CustomerId { get; set; }
    //    public int ServiceId { get; set; }
    //    public int ProviderId { get; set; }
    //    public int Rating { get; set; }
    //    public string Comment { get; set; }
    //    public bool IsVerified { get; set; }
    //    public DateTime CreatedDate { get; set; }
    //}
}