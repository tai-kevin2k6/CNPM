using CNPM.Api.Dtos;
using CNPM.DTOs;
using CNPM.Entities;
using CNPM.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static CNPM.Api.Dtos.Reponse; // Lưu ý sửa typo Reponse -> Response nếu cần

namespace CNPM.Api.Services.HubManage
{
    public class HubService : IHubService
    {
        private readonly ApplicationDbContext _context;

        public HubService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<HubOptionDto>> GetHubOptionsForOwnerAsync(int userId)
        {
            return await _context.PickupHubs
                .AsNoTracking()
                .Where(h => h.OwnerId == userId)
                // Bỏ comment dòng Where cũ để lấy tất cả trạng thái
                .Select(h => new HubOptionDto
                {
                    Id = h.Id,
                    // Logic: Nếu khóa thì cộng chuỗi, nếu reject thì cộng chuỗi, còn lại giữ nguyên
                    Name = (h.Status == "locked" || h.Status == "closed") ? h.Name + " (bị khóa)" :
                           h.Status == "reject" ? h.Name + " (bị từ chối)" :
                           h.Name
                })
                .ToListAsync();
        }

        public async Task<PagedResult<OrderManagementDto>> GetOrdersForHubOwnerAsync(int userId, OrderFilterRequest request)
        {
            // 1. Lấy danh sách Hub (để dùng cho Dropdown VÀ để lọc dữ liệu)
            var hubOptions = await GetHubOptionsForOwnerAsync(userId);

            // Lấy list ID để query
            var ownedHubIds = hubOptions.Select(x => x.Id).ToList();

            // Nếu user không quản lý kho nào
            if (!ownedHubIds.Any())
            {
                // Trả về rỗng nhưng VẪN TRẢ VỀ OPTION (để dropdown không bị null)
                return new PagedResult<OrderManagementDto>(new List<OrderManagementDto>(), 0, hubOptions);
            }

            // 2. Query dữ liệu (Phần này giữ nguyên logic lọc cũ của bạn)
            var query = _context.Orders
                .Include(o => o.PickupHub)
                .Include(o => o.User)
                .Where(o => ownedHubIds.Contains(o.PickupHubId));

            if (!string.IsNullOrEmpty(request.OrderKeyword))
                query = query.Where(o => o.Id.ToString() == request.OrderKeyword);

            if (!string.IsNullOrEmpty(request.HubNameKeyword) && request.HubNameKeyword != "all")
                query = query.Where(o => o.PickupHub != null && o.PickupHub.Name == request.HubNameKeyword);

            if (!string.IsNullOrEmpty(request.Status) && request.Status != "all")
                query = query.Where(o => o.Status == request.Status);

            // 3. Đếm & Lấy dữ liệu
            int totalRecords = await query.CountAsync();

            var items = await query
                .OrderByDescending(o => o.Id)
                .Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(o => new OrderManagementDto
                {
                    Id = o.Id,
                    HubId = o.PickupHubId,
                    HubName = o.PickupHub != null ? o.PickupHub.Name : "Chưa gán kho",
                    ReceiverName = o.User != null ? o.User.FullName : "Khách vãng lai",
                    ReceiverPhone = o.PhoneNumber,
                    Status = o.Status
                })
                .ToListAsync();

            // 4. RETURN: Nhét cả Items, TotalRecords VÀ HubOptions vào chung 1 gói
            return new PagedResult<OrderManagementDto>(items, totalRecords, hubOptions);
        }
        public async Task<List<HubSummaryDto>> GetPendingHubsAsync()
        {
            // Lấy tất cả Hub có trạng thái là Pending
            return await _context.PickupHubs
                .AsNoTracking()
                .Where(h => h.Status == "pending")
                .OrderByDescending(h => h.Id) // Cái mới nhất lên đầu
                .Select(h => new HubSummaryDto
                {
                    Id = h.Id,
                    Name = h.Name,
                    Address = h.Address
                })
                .ToListAsync();
        }
        //cần tối ưu 
        public async Task<List<HubSummaryDto>> GetAllHubsAsync()
        {
            return await _context.PickupHubs
                .AsNoTracking()
                .Include(h => h.User) // <--- KẾT NỐI (JOIN) BẢNG USER TẠI ĐÂY
                .OrderByDescending(h => h.Id)
                .Where(h => h.Status != "pending")
                .Where(h => h.Status != "reject")
                .Select(h => new HubSummaryDto
                {
                    Id = h.Id,
                    Name = h.Name,
                    Address = h.Address,
                    Status = h.Status,

                    // Lấy dữ liệu từ bảng User đã Include
                    // Dùng dấu ? (h.Owner?) để tránh lỗi nếu Hub chưa có chủ
                    OwnerName = h.User != null ? h.User.FullName : "Chưa có chủ",
                    OwnerPhone = h.User != null ? h.User.PhoneNumber : ""
                })
                .ToListAsync();
        }

        public async Task<HubDetailDto> GetHubDetailForAdminAsync(int hubId)
        {
            // 1. Query 1 lần duy nhất lấy cả Hub lẫn User
            var hub = await _context.PickupHubs
                .AsNoTracking()
                .Include(h => h.User) // <--- Key logic: Load luôn User dựa trên ForeignKey OwnerId
                .FirstOrDefaultAsync(h => h.Id == hubId);

            // 2. Kiểm tra null
            if (hub == null) return null;

            // 3. Map dữ liệu trả về
            return new HubDetailDto
            {
                Name = hub.Name,
                Address = hub.Address,
                Status = hub.Status, // Class PickupHub của bạn Status là string nên không cần .ToString()
                OpeningHours = hub.OpeningHours,
                ClosingHours = hub.ClosingHours,

                // Thông tin chủ sở hữu (Lấy từ navigation property 'User')
                OwnerId = hub.OwnerId,

                // Kiểm tra null bằng ?. (User có thể null nếu data cũ bị lỗi)
                // Giả sử class User của bạn có trường FullName hoặc Name
                OwnerName = hub.User?.FullName ?? "Không xác định",
                PhoneNumber = hub.User?.PhoneNumber ?? ""
            };
        }

        // Nhớ bổ sung hàm này vào Interface IHubManageService trước nhé
        public async Task<bool> ApproveHubAsync(ApproveHubRequest request)
        {
            // 1. Tìm Hub trong Database
            var hub = await _context.PickupHubs.FindAsync(request.HubId);

            // 2. Kiểm tra tồn tại
            if (hub == null)
            {
                throw new Exception("Không tìm thấy điểm nhận hàng này.");
            }

            // 4. Xử lý Logic chính
            if (request.IsLocked == true)
            {
                // TRƯỜNG HỢP DUYỆT
                hub.Status = "reject";
                if (request.Reason != null)
                    hub.Reason = request.Reason; // Lưu lý do vào DB
                else hub.Reason = null;
            }
            else
            {
                // TRƯỜNG HỢP TỪ CHỐI
                hub.Status = "active";
                if (request.Reason != null)
                    hub.Reason = request.Reason; // Lưu lý do vào DB
                else hub.Reason = null;
            }

            // 5. Lưu thay đổi
            _context.PickupHubs.Update(hub);
            var result = await _context.SaveChangesAsync();

            return result > 0; // Trả về true nếu lưu thành công
        }

        public async Task<bool> LockHubAsync(ApproveHubRequest request)
        {
            // 1. Tìm Hub trong Database
            var hub = await _context.PickupHubs.FindAsync(request.HubId);

            // 2. Kiểm tra tồn tại
            if (hub == null)
            {
                throw new Exception("Không tìm thấy điểm nhận hàng này.");
            }

            // 4. Xử lý Logic chính
            if (request.IsLocked == true)
            {
                // TRƯỜNG HỢP KHÓA
                hub.Status = "closed";
                if (request.Reason != null)
                    hub.Reason = request.Reason; // Lưu lý do vào DB
                else hub.Reason = null;
            }
            else
            {
                // TRƯỜNG HỢP TỪ CHỐI
                hub.Status = "active";
                if (request.Reason != null)
                    hub.Reason = request.Reason; // Lưu lý do vào DB
                else hub.Reason = null;
            }

            // 5. Lưu thay đổi
            _context.PickupHubs.Update(hub);
            var result = await _context.SaveChangesAsync();

            return result > 0; // Trả về true nếu lưu thành công
        }

        public async Task<bool> UpdateOrderStatusAsync(UpdateOrderStatusRequest request)
        {
            // 1. Tìm đơn hàng
            var order = await _context.Orders.FindAsync(request.OrderId);
            if (order == null)
            {
                throw new Exception("Không tìm thấy đơn hàng.");
            }

            // 2. Cập nhật trạng thái
            order.Status = request.NewStatus;

            // 3. Lưu vào DB
            _context.Orders.Update(order);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}