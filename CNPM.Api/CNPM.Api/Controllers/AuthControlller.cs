using CNPM.Entities;
using CNPM.DTOs;
using CNPM.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CNPM.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration; // Cần cái này để lấy Key từ appsettings

        public AuthController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("user-register")]
        public async Task<IActionResult> UserRegister([FromBody] RegisterRequest request)
        {
            if (await _context.Users.AnyAsync(u => u.Username == request.Username))
                return BadRequest("Tài khoản đã tồn tại");

            var user = new User
            {
                Username = request.Username,
                Password = request.Password,
                PhoneNumber = request.PhoneNumber,
                FullName = request.FullName,
                Role = "Customer"
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok("Đăng ký thành công!");
        }

        [HttpPost("hub-register")]
        public async Task<IActionResult> HubRegister([FromBody] RegisterRequest request)
        {
            if (await _context.Users.AnyAsync(u => u.Username == request.Username))
                return BadRequest("Tài khoản đã tồn tại");

            var user = new User
            {
                Username = request.Username,
                Password = request.Password,
                PhoneNumber = request.PhoneNumber,
                FullName = request.FullName,
                Role = "HubOwner"
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok("Đăng ký thành công!");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == request.Username && u.Password == request.Password);

            if (user == null)
                return Unauthorized("Sai tên đăng nhập hoặc mật khẩu");

            // === TẠO TOKEN JWT ===
            var token = CreateToken(user);

            return Ok(new { Token = token });
        }

        // Hàm phụ trợ để sinh Token
        private string CreateToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), // Lưu ID user
                new Claim(ClaimTypes.Name, user.Username),               // Lưu tên
                new Claim(ClaimTypes.Role, user.Role)                    // Lưu quyền (Customer/HubOwner)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1), // Token sống 1 ngày
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}