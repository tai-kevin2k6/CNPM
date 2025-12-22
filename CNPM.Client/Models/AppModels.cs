namespace CNPM.Client.Models
{
    // --- KHU VỰC AUTH (Đăng nhập/Đăng ký) ---
    // Khớp với AuthController.cs
    public class RegisterRequest
    {
        public string PhoneNumber { get; set; } // Backend dùng PhoneNumber
        public string Password { get; set; }
        public string Password_2 { get; set; }
        public string FullName { get; set; }
        public string Role { get; set; } = "User"; // Mặc định là User
    }

    public class LoginRequest
    {
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
    }

    public class LoginResponse
    {
        public string Token { get; set; }
    }

    // --- KHU VỰC ĐƠN HÀNG (Order) ---
    // Khớp với OrderController.cs
    public class OrderDto
    {
        public int Id { get; set; }
        public int PickupHubId { get; set; }
        public string PickupHubName { get; set; }
        public string Status { get; set; }
        public string PhoneNumber { get; set; }
    }

    // Dùng để tạo đơn hàng mới (nếu cần sau này)
    public class CreateOrderRequest
    {
        public int PickupHubId { get; set; }
        public int PickupUserId { get; set; }
        public string PhoneNumber { get; set; }
    }

    // --- KHU VỰC ĐIỂM GIAO NHẬN (PickupHub) ---
    // Khớp với PickupHubController.cs
    public class PickupHubDto
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string OpeningHours { get; set; }
        public string ClosingHours { get; set; }
        public string Status { get; set; }
    }

    public class CreateHubRequest
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string OpeningHours { get; set; }
        public string ClosingHours { get; set; }
    }

}