using CNPM.Api.Dtos;
using CNPM.Api.Services;
using CNPM.DTOs;
using CNPM.Entities;
using CNPM.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CNPM.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PickupHubController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IPickupHubRepository _pickupHubRepository;

        public PickupHubController(ApplicationDbContext context, IPickupHubRepository pickupHubRepository)
        {
            _pickupHubRepository = pickupHubRepository;
            _context = context;
        }

        // US04: Chủ điểm đăng ký điểm nhận hàng
        [HttpPost("register-hub")]
        [Authorize(Roles = "HubOwner")] // 2. Bắt buộc phải có Token và quyền HubOwner
        public async Task<IActionResult> RegisterHub([FromBody] CreateHubRequest request)
        {
            // --- BƯỚC QUAN TRỌNG: LẤY ID TỪ TOKEN ---
            // Khi đăng nhập, ta đã nhét Id vào ClaimTypes.NameIdentifier
            // Bây giờ ta lôi nó ra.
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdString))
            {
                return Unauthorized("Không tìm thấy thông tin người dùng trong Token");
            }

            int ownerIdFromToken = int.Parse(userIdString);

            // (Tùy chọn) Kiểm tra lại trong DB cho chắc ăn
            var owner = await _context.Users.FindAsync(ownerIdFromToken);
            if (owner == null) return BadRequest("User không tồn tại");

            // --- TẠO DATA ---
            var hub = new PickupHub
            {
                Name = request.Name,
                Address = request.Address,
                OpeningHours = request.OpeningHours,
                OwnerId = ownerIdFromToken // 3. Gán ID lấy từ Token vào đây
            };

            _context.PickupHubs.Add(hub);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Đăng ký điểm nhận hàng thành công", HubId = hub.Id });
        }

        // US03: Khách hàng xem thông tin chi tiết điểm nhận hàng
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPickupHubById(int id)
        {
            var hubEntity = await _pickupHubRepository.GetHubDetailByIdAsync(id); // Lấy Entity gốc

            if (hubEntity == null) return NotFound();

            // Mapping thủ công (hoặc dùng AutoMapper)
            var resultDto = new PickupHubDetailDto
            {
                Id = hubEntity.Id,
                Name = hubEntity.Name,
                Address = hubEntity.Address,
                OpeningHours = hubEntity.OpeningHours,
                Owner = new HubOwnerPublicDto
                {
                    // Chỉ lấy FullName, các trường khác sẽ null hoặc không tồn tại trong DTO này
                    FullName = hubEntity.Owner.FullName
                }
            };

            return Ok(resultDto);
        }

        // API phụ: Lấy danh sách tất cả Hub để xem
        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _context.PickupHubs.ToListAsync());
        }
    }
}