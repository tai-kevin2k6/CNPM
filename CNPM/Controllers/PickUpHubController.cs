using CNPM.Entities;
using CNPM.DTOs;
using CNPM.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CNPM.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PickupHubController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PickupHubController(ApplicationDbContext context)
        {
            _context = context;
        }

        // US04: Chủ điểm đăng ký điểm nhận hàng
        [HttpPost("register-hub")]
        public async Task<IActionResult> RegisterHub([FromBody] CreateHubRequest request)
        {
            // Kiểm tra xem User có phải là HubOwner không
            var owner = await _context.Users.FindAsync(request.OwnerId);
            if (owner == null || owner.Role != "HubOwner")
                return BadRequest("User không tồn tại hoặc không phải là Chủ điểm");

            var hub = new PickupHub
            {
                Name = request.Name,
                Address = request.Address,
                OpeningHours = request.OpeningHours,
                OwnerId = request.OwnerId
            };

            _context.PickupHubs.Add(hub);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Đăng ký điểm nhận hàng thành công", HubId = hub.Id });
        }

        // US03: Khách hàng xem thông tin chi tiết điểm nhận hàng
        [HttpGet("{id}")]
        public async Task<IActionResult> GetHubDetail(int id)
        {
            var hub = await _context.PickupHubs
                .Include(h => h.Owner) // Lấy kèm thông tin chủ nếu cần
                .FirstOrDefaultAsync(h => h.Id == id);

            if (hub == null) return NotFound("Không tìm thấy điểm nhận hàng");

            return Ok(hub);
        }

        // API phụ: Lấy danh sách tất cả Hub để xem
        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _context.PickupHubs.ToListAsync());
        }
    }
}