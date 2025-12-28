namespace CNPM.Api.Dtos
{
    public class Reponse
    {
        // 1. Dữ liệu gửi lên (Payload)
        public class OrderFilterRequest
        {
            public string? HubNameKeyword { get; set; } = "all"; // Tìm kiếm theo Tên Kho
            public string? Status { get; set; } = "all";    // Lọc theo trạng thái (Ready, PickedUp...)
            public int PageIndex { get; set; } = 1;     // Trang số mấy
            public int PageSize { get; set; } = 10;     // Lấy bao nhiêu dòng
        }

        // 2. Dữ liệu trả về (Kết quả hiển thị ra bảng)
        public class OrderManagementDto
        {
            public int Id { get; set; }
            public int OrderCode { get; set; }    // Mã đơn
            public string HubName { get; set; }      // Tên kho đang giữ đơn này
            public string ReceiverName { get; set; }
            public string ReceiverPhone { get; set; }
            public string Status { get; set; }
        }

        // 3. Wrapper phân trang (để FE biết tổng số trang)
        public class PagedResult<T>
        {
            public PagedResult(List<T> items, int totalRecords)
            {
                Items = items;
                TotalRecords = totalRecords;
            }
            public List<T> Items { get; set; }
            public int TotalRecords { get; set; }
        }

        // 1. DTO cho danh sách (Hiện ở bảng bên ngoài)
        public class HubSummaryDto
        {
            public int Id { get; set; }
            public string Name { get; set; }        // Tên kho
            public string Address { get; set; }     // Địa chỉ ngắn gọn
        }

        // 2. DTO cho chi tiết (Hiện khi bấm vào xem)
        public class HubDetailDto
        {
            public string? Name { get; set; }
            public string? Address { get; set; }
            public string? OpeningHours { get; set; }
            public string? ClosingHours { get; set; }
            public string? Status { get; set; }

            // Thông tin người sở hữu (để Admin liên hệ nếu cần)
            public int OwnerId { get; set; }
        }
    }
}

