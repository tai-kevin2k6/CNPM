namespace CNPM.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Password { get; set; } 
        public string PhoneNumber { get; set; }
        public string FullName { get; set; }
        public string Role { get; set; } // "Customer" , "HubOwner"
    }
}