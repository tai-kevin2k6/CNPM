namespace CNPM.Entities
{
    public class Order
    {
        public int Id { get; set; }
        public string OrderCode { get; set; } // Mã vận đơn
        public string Status { get; set; } // Ví dụ: "DangGiao", "DaDenTram", "HoanThanh"
        public string SenderPhone { get; set; } // Số điện thoại người gửi/nhận để tra cứu

        // Liên kết với điểm nhận hàng
        public int? PickupHubId { get; set; }
        public PickupHub PickupHub { get; set; }
    }
}