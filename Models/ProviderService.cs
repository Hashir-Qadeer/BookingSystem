namespace BookingSystem.Models
{
    public class ProviderService
    {
        public int ProviderServiceId { get; set; }
        public int ProviderId { get; set; }
        public int ServiceId { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
