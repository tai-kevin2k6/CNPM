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

    // Models/OrderFilterRequest.cs
    public class OrderFilterRequest
    {
        public string? OrderKeyword { get; set; } = null;
        public string? HubNameKeyword { get; set; } = "all";
        public string Status { get; set; } = "all";
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    // Models/OrderManagementDto.cs
    public class OrderManagementDto
    {
        public int Id { get; set; }
        public int HubId { get; set; }
        public string HubName { get; set; }
        public string ReceiverName { get; set; }
        public string ReceiverPhone { get; set; }
        public string Status { get; set; } // "pending", "arrived", "picked_up", "overdue"
    }

    // Models/PagedResult.cs
    public class PagedResult<T>
    {
        public List<T> Items { get; set; } = new List<T>();
        public int TotalRecords { get; set; }
        public List<HubOptionDto> HubOptions { get; set; } = new List<HubOptionDto>();
    }

    // Models/HubOptionDto.cs (Cho dropdown chọn kho)
    public class HubOptionDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

}