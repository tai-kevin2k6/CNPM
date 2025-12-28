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
                .Select(h => new HubOptionDto
                {
                    Id = h.Id,
                    Name = h.Name
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
                .Select(h => new HubSummaryDto
                {
                    Id = h.Id,
                    Name = h.Name,
                    Address = h.Address
                })
                .ToListAsync();
        }

        public async Task<HubDetailDto> GetHubDetailForAdminAsync(int hubId)
        {
            // Lấy chi tiết 1 Hub
            var hub = await _context.PickupHubs
                .AsNoTracking()
                .Include(h => h.User) // Quan trọng: Phải lấy thông tin chủ shop
                .FirstOrDefaultAsync(h => h.Id == hubId);

            if (hub == null) return null;

            return new HubDetailDto
            {
                Name = hub.Name,
                Address = hub.Address,
                Status = hub.Status.ToString(), 
                OpeningHours = hub.OpeningHours,
                ClosingHours = hub.ClosingHours,

                // Map thông tin chủ sở hữu
                OwnerId = hub.OwnerId
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
                hub.Status = "closed";
                hub.Reason = request.Reason; // Lưu lý do vào DB
            }
            else
            {
                // TRƯỜNG HỢP TỪ CHỐI
                hub.Status = "active";
                hub.Reason = null; // Xóa lý do cũ (nếu có)
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