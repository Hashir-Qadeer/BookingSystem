namespace BookingSystem.Models
{
    public class Payment
    {
        public int PaymentId { get; set; }
        public int AppointmentId { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } // CreditCard, DebitCard, Cash, Online
        public string TransactionId { get; set; }
        public string PaymentStatus { get; set; } // Pending, Completed, Failed, Refunded
        public DateTime? PaymentDate { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
