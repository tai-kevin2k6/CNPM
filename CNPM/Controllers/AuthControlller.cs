using CNPM.Entities;
using CNPM.DTOs;
using CNPM.Infrastructure.Data; // Sử dụng namespace Data mới
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CNPM.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AuthController(ApplicationDbContext context)
        {
            _context = context;
        }

        // US01: Đăng ký tài khoản
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (await _context.Users.AnyAsync(u => u.Username == request.Username))
                return BadRequest("Tài khoản đã tồn tại");

            var user = new User
            {
                Username = request.Username,
                Password = request.Password, // Lưu ý: Nên hash password ở đây
                PhoneNumber = request.PhoneNumber,
                FullName = request.FullName,
                Role = request.Role
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok("Đăng ký thành công!");
        }

        // US01: Đăng nhập
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == request.Username && u.Password == request.Password);

            if (user == null)
                return Unauthorized("Sai tên đăng nhập hoặc mật khẩu");

            // Trả về thông tin user để frontend lưu lại (thay vì JWT phức tạp)
            return Ok(user);
        }
    }
}