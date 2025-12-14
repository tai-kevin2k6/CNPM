namespace CNPM.DTOs
{
    // DTO cho đăng ký (US01)
    public class RegisterRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        public string FullName { get; set; }
    }

    // DTO cho đăng nhập (US01)
    public class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    // DTO cho đăng ký điểm nhận hàng (US04)
    public class CreateHubRequest
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string OpeningHours { get; set; }
    }

    // DTO tạo đơn hàng (Để test US02)
    public class CreateOrderRequest
    {
        public string OrderCode { get; set; }
        public string SenderPhone { get; set; }
        public int PickupHubId { get; set; }
    }
}