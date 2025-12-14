namespace CNPM.Entities
{
    public class PickupHub
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string OpeningHours { get; set; }

        // Liên kết với chủ sở hữu (User)
        public int OwnerId { get; set; }
        public User Owner { get; set; }
    }
}