namespace CNPM.DTOs
{
    public class RegisterRequest
    {
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public string Role { get; set; }
    }

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

    public class ApproveHubRequest
    {
        public int HubId { get; set; }
        public bool IsLocked { get; set; }
        public string? Reason { get; set; }
    }

    public class ApproveUserRequest
    {
        public int UserId { get; set; }
        public bool IsLocked { get; set; } // true = Khóa, false = Mở
        public string? Reason { get; set; }
    }
}