namespace CNPM.Api.Dtos
{
    public class PickupHubDetailDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string OpeningHours { get; set; }

        // Thay vì trả về cả object Owner, ta chỉ trả về object rút gọn hoặc vài trường cần thiết
        public HubOwnerPublicDto Owner { get; set; }
    }

    public class HubOwnerPublicDto
    {
        // Chỉ hiển thị tên hiển thị, giấu username/password/sđt
        public string FullName { get; set; }
        // Có thể thêm Email nếu cần liên hệ công việc, nhưng tuyệt đối không thêm Password
    }
}
