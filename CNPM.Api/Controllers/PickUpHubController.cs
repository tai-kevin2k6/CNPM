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

        [HttpPost("register-hub")]
        [Authorize(Roles = "HubOwner")]
        public async Task<IActionResult> RegisterHub([FromBody] CreateHubRequest request)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdString))
            {
                return Unauthorized("Không tìm thấy thông tin người dùng trong Token");
            }

            int ownerIdFromToken = int.Parse(userIdString);

            var owner = await _context.Users.FindAsync(ownerIdFromToken);
            if (owner == null) return BadRequest("User không tồn tại");

            var hub = new PickupHub
            {
                Name = request.Name,
                Address = request.Address,
                OpeningHours = request.OpeningHours,
                ClosingHours = request.ClosingHours,
                Status = "Pending",
                OwnerId = ownerIdFromToken 
            };

            _context.PickupHubs.Add(hub);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Đã gửi đơn đăng kí điểm", HubId = hub.Id });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPickupHubById(int id)
        {
            var hubEntity = await _pickupHubRepository.GetHubDetailByIdAsync(id);

            if (hubEntity == null) return NotFound();

            var resultDto = new PickupHubDetailDto
            {
                Name = hubEntity.Name,
                Address = hubEntity.Address,
                OpeningHours = hubEntity.OpeningHours,
                ClosingHours = hubEntity.ClosingHours,
                Status = hubEntity.Status
            };

            return Ok(resultDto);
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            var hubs = await _context.PickupHubs
                .Select(h => new PickupHubDetailDto
                {
                    Name = h.Name,
                    Address = h.Address,   
                    OpeningHours = h.OpeningHours,
                    ClosingHours = h.ClosingHours,
                    Status = h.Status
                })
                .ToListAsync();

            return Ok(hubs);
        }
    }
}