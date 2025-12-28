using CNPM.Api.Dtos;
using CNPM.Api.Services;
using CNPM.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CNPM.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "admin")]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IServiceWrapper _service;

        public UserController(ApplicationDbContext context, IServiceWrapper service)
        {
            _context = context;
            _service = service;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDetailUser(int id)
        {
            // 1. Tìm user trong database
            var user = await _context.Users
                .AsNoTracking() // Tối ưu hiệu suất vì chỉ đọc, không sửa
                .FirstOrDefaultAsync(u => u.Id == id);

            // 2. Kiểm tra nếu không tìm thấy
            if (user == null)
            {
                return NotFound(new { Message = "Không tìm thấy người dùng này." });
            }

            // 3. Trả về kết quả
            var result = new
            {
                Id = user.Id,
                FullName = user.FullName,
                PhoneNumber = user.PhoneNumber
            };

            return Ok(result);
        }


    }
}
