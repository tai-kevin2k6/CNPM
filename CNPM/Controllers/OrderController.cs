using CNPM.Entities;
using CNPM.DTOs;
using CNPM.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CNPM.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public OrderController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Tạo đơn hàng giả (để có dữ liệu mà test)
        [HttpPost("create-test")]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
        {
            var order = new Order
            {
                OrderCode = request.OrderCode,
                Status = "DangVanChuyen",
                SenderPhone = request.SenderPhone,
                PickupHubId = request.PickupHubId
            };
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            return Ok(order);
        }

        // US02: Tra cứu đơn hàng bằng Mã đơn hoặc Số điện thoại
        [HttpGet("tracking")]
        public async Task<IActionResult> TrackingOrder([FromQuery] string keyword)
        {
            // Tìm theo Code hoặc SĐT
            var orders = await _context.Orders
                .Include(o => o.PickupHub) // Kèm thông tin Hub để khách biết đâu mà lấy
                .Where(o => o.OrderCode == keyword || o.SenderPhone == keyword)
                .ToListAsync();

            if (orders == null || orders.Count == 0)
                return NotFound("Không tìm thấy đơn hàng nào.");

            return Ok(orders);
        }
    }
}