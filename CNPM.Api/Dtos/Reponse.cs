using Microsoft.AspNetCore.Http.Features;

namespace CNPM.Api.Dtos
{
    public class Reponse
    {
        // 1. Dữ liệu gửi lên (Payload)
        public class OrderFilterRequest
        {
            public string? OrderKeyword { get; set; } = null;
            public string? HubNameKeyword { get; set; } = "all"; // Tìm kiếm theo Tên Kho
            public string? Status { get; set; } = "all";    // Lọc theo trạng thái (Ready, PickedUp...)
            public int PageIndex { get; set; } = 1;     // Trang số mấy
            public int PageSize { get; set; } = 10;     // Lấy bao nhiêu dòng
        }

        // 2. Dữ liệu trả về (Kết quả hiển thị ra bảng)
        public class OrderManagementDto
        {
            public int Id { get; set; }
            public int HubId { get; set; } 
            public string HubName { get; set; }      // Tên kho đang giữ đơn này
            public string ReceiverName { get; set; }
            public string ReceiverPhone { get; set; }
            public string Status { get; set; }
        }

        // 3. Wrapper phân trang (để FE biết tổng số trang)
        // 3. Wrapper phân trang (SỬA LẠI)
        public class PagedResult<T>
        {
            public List<T> Items { get; set; }
            public int TotalRecords { get; set; }
            public List<HubOptionDto>? HubOptions { get; set; }

            // Constructor đầy đủ
            public PagedResult(List<T> items, int totalRecords, List<HubOptionDto>? hubOptions = null)
            {
                Items = items;
                TotalRecords = totalRecords;
                HubOptions = hubOptions ?? new List<HubOptionDto>();
            }
        }

        // 1. DTO cho danh sách (Hiện ở bảng bên ngoài)
        public class HubSummaryDto
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Address { get; set; }

            // --- CÁC TRƯỜNG MỚI ---
            public string Status { get; set; }      // Trạng thái Hub
            public string OwnerName { get; set; }   // Tên chủ sở hữu
            public string OwnerPhone { get; set; }  // SĐT chủ sở hữu
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
            public string OwnerName { get; set; }
            public string PhoneNumber { get; set; }
        }

        public class HubOptionDto
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }
    }
}

