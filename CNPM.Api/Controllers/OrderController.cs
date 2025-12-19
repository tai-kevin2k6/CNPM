using CNPM.DTOs;
using CNPM.Entities;
using CNPM.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CNPM.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public OrderController(ApplicationDbContext context)
        {
            _context = context;
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
    }
}