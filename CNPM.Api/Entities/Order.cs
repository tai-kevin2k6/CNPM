namespace CNPM.Entities
{
    public class Order
    {
        public int? Id { get; set; }
        public int? PickupHubId { get; set; }
        public int? PickupUserId { get; set; }
        public string Status { get; set; } 
        public string PhoneNumber { get; set; }
    }
}