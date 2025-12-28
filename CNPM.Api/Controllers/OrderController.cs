using CNPM.Api.Dtos;
using CNPM.Api.Services;
using CNPM.Api.Services.HubManage;
using CNPM.DTOs;
using CNPM.Entities;
using CNPM.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using static CNPM.Api.Dtos.Reponse;

namespace CNPM.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IServiceWrapper _service;

        public OrderController(ApplicationDbContext context, IServiceWrapper service)
        {
            _context = context;
            _service = service;
        }

        [HttpPost("create-order")]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
        {
            var order = new Order
            {
                PickupHubId = request.PickupHubId,
                PickupUserId = request.PickupUserId,
                Status = "DangVanChuyen",
                PhoneNumber = request.PhoneNumber,
            };
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            return Ok(order);
        }

        [HttpGet("tracking")]
        public async Task<IActionResult> TrackingOrder([FromQuery] string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return BadRequest("Vui lòng nhập từ khóa tìm kiếm.");

            var query = from o in _context.Orders
                        join h in _context.PickupHubs on o.PickupHubId equals h.Id
                        select new
                        {
                            o.Id,
                            o.PickupHubId,
                            o.Status,
                            o.PhoneNumber,
                            PickupHubName = h.Name
                        };

            // 2. Lọc dữ liệu
            bool isNumeric = int.TryParse(keyword, out int orderId);
            query = query.Where(x => x.PhoneNumber == keyword || (isNumeric && x.Id == orderId));

            var orders = await query.ToListAsync();

            if (orders == null || !orders.Any())
                return NotFound("Không tìm thấy đơn hàng nào.");

            return Ok(orders);
        }

        [HttpPost("update-status")] // Hoặc dùng [HttpPut]
        public async Task<IActionResult> UpdateStatus([FromBody] UpdateOrderStatusRequest request)
        {
            try
            {
                // Kiểm tra status gửi lên có nằm trong danh sách cho phép không
                var allowedStatuses = new[] { "pending", "ready", "pickedup", "cancelled" };
                if (!allowedStatuses.Contains(request.NewStatus.ToLower()))
                {
                    return BadRequest(new { Success = false, Message = "Trạng thái không hợp lệ." });
                }

                await _service.HubManage.UpdateOrderStatusAsync(request); // Nhớ gọi đúng Service của bạn

                return Ok(new { Success = true, Message = "Cập nhật trạng thái thành công." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Success = false, Message = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("manage-orders")]
        public async Task<IActionResult> GetOrdersForManagement([FromQuery] OrderFilterRequest request)
        {
            try
            {
                // 1. Lấy claim ra trước
                var idClaim = User.FindFirst(ClaimTypes.NameIdentifier);

                // 2. Kiểm tra xem có lấy được không
                if (idClaim == null)
                {
                    // Nếu không tìm thấy ID trong token -> Trả về lỗi 401 hoặc 403
                    return Unauthorized("Token không hợp lệ hoặc thiếu thông tin ID.");
                }

                // 3. Parse sang số an toàn
                var id = int.Parse(idClaim.Value);

                // --- Sau đó dùng biến 'id' này để query ---
                return  Ok(await _service.HubManage.GetOrdersForHubOwnerAsync(id, request));
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Lỗi Server: " + ex.Message);
            }
        }
    }
}