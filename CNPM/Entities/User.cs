namespace CNPM.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; } // Lưu ý: Thực tế nên mã hóa, ở đây để plain text cho đơn giản
        public string PhoneNumber { get; set; }
        public string FullName { get; set; }
        public string Role { get; set; } // "Customer" hoặc "HubOwner"
    }
}