using CNPM.Api.Services;
using CNPM.Api.Services.HubManage;
using CNPM.DTOs;
using CNPM.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Writers;
using static CNPM.Api.Dtos.Reponse;

namespace CNPM.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "admin")]
    public class AdminController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IServiceWrapper _service;
        public AdminController(ApplicationDbContext context, IServiceWrapper service)
        {
            _context = context;
            _service = service;
        }

        [HttpGet("hub-pending")]
        public async Task<IActionResult> GetPendingList()
        {
            var result = await _service.HubManage.GetPendingHubsAsync();
            return Ok(result);
        }

        [HttpGet("hub/{id}")]
        public async Task<IActionResult> GetDetail(int id)
        {
            var result = await _service.HubManage.GetHubDetailForAdminAsync(id);

            if (result == null)
                return NotFound(new { Message = "Không tìm thấy điểm nhận hàng này." });

            return Ok(result);
        }

        [HttpPost("approve-pending")]
        public async Task<IActionResult> ProcessRequest([FromBody] ApproveHubRequest request)
        {
            try
            {
                await _service.HubManage.ApproveHubAsync(request);

                // Trả về thông báo tương ứng để Frontend dễ hiển thị
                string message = request.IsLocked ? "Đã duyệt thành công." : "Đã từ chối yêu cầu.";

                return Ok(new { Success = true, Message = message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Success = false, Message = ex.Message });
            }
        }

        [HttpGet("all-users")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _service.UserManage.GetAllUsersAsync();
            return Ok(users);
        }

        [HttpPost("users/lock")]
        public async Task<IActionResult> LockUser([FromBody] ApproveUserRequest request)
        {
            try
            {
                await _service.UserManage.ApproveUser(request);
                var msg = request.IsLocked ? "Đã khóa tài khoản." : "Đã mở khóa tài khoản.";
                return Ok(new { Success = true, Message = msg });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Success = false, Message = ex.Message });
            }
        }

        // ==========================================
        // KHU VỰC 2: QUẢN LÝ HUBS (ĐIỂM NHẬN HÀNG)
        // ==========================================

        [HttpGet("all-hubs")] // Lấy danh sách Hub đang chạy (hoặc đang bị khóa)
        public async Task<IActionResult> GetActiveHubs()
        {
            var hubs = await _service.HubManage.GetAllHubsAsync();
            return Ok(hubs);
        }

        [HttpPost("hubs/lock")]
        public async Task<IActionResult> LockHub([FromBody] ApproveHubRequest request)
        {
            try
            {
                await _service.HubManage.ApproveHubAsync(request);
                var msg = request.IsLocked ? "Đã khóa điểm nhận hàng." : "Đã mở lại hoạt động.";
                return Ok(new { Success = true, Message = msg });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Success = false, Message = ex.Message });
            }
        }

    }
}