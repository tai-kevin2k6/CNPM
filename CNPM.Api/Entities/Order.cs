using System.ComponentModel.DataAnnotations.Schema;

namespace CNPM.Entities
{
    public class Order
    {
        public int Id { get; set; }
        public int PickupHubId { get; set; }
        [ForeignKey("PickupHubId")]
        public virtual PickupHub PickupHub { get; set; }
        public int? PickupUserId { get; set; }

        [ForeignKey("PickupUserId")]
        public virtual User User { get; set; }
        public string Status { get; set; } 
        public string PhoneNumber { get; set; }
    }
}