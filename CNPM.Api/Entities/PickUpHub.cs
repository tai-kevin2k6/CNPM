namespace CNPM.Entities
{
    public class PickupHub
    {
        public int Id { get; set; }
        public int OwnerId { get; set; }
        public string Address { get; set; }
        public string OpeningHours { get; set; }
        public string ClosingHours { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
    }
}