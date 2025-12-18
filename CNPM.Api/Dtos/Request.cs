namespace CNPM.DTOs
{
    // DTO cho đăng ký (US01)
    public class RegisterRequest
    {
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public string Role { get; set; }
    }

    // DTO cho đăng nhập (US01)
    public class LoginRequest
    {
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
    }

    public class CreateHubRequest
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string OpeningHours { get; set; }
        public string ClosingHours { get; set; }
    }

    public class CreateOrderRequest
    {
        public string PhoneNumber { get; set; }
        public int PickupHubId { get; set; }
        public int? PickupUserId { get; set; }
        public string Status { get; set; }

    }
}