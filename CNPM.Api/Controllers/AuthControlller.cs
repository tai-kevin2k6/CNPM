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
        private readonly IConfiguration _configuration;

        public AuthController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> UserRegister([FromBody] RegisterRequest request)
        {
            if ((request.PhoneNumber).Length < 10) return BadRequest("SĐT phải dài hơn 10 kí tự");
            if (await _context.Users.AnyAsync(u => u.PhoneNumber == request.PhoneNumber))
                return BadRequest("SĐT đã tồn tại");

            var user = new User
            {
                Password = request.Password,
                PhoneNumber = request.PhoneNumber,
                FullName = request.FullName,
                Role = request.Role,
                };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok("Đăng ký thành công!");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.PhoneNumber))
                return Unauthorized("Chưa nhập SĐT");
            if (string.IsNullOrWhiteSpace(request.Password))
                return Unauthorized("Chưa nhập mật khẩu");
            if (request.PhoneNumber.Length < 10)
                return Unauthorized("Số điện thoại đăng nhập không hợp lệ");
            if (request.Password.Length < 6)
                return Unauthorized("Mật khẩu có độ dài không hợp lệ");

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.PhoneNumber == request.PhoneNumber && u.Password == request.Password);
            if (user.Status == "locked")
                return BadRequest(new
                {
                    Success = false,
                    Message = $"Tài khoản đã bị khóa."
                });
            if (user == null)
                return Unauthorized("Sai SĐT hoặc mật khẩu");

            var token = CreateToken(user);

            return Ok(new { Token = token });
        }

        private string CreateToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),  
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.Role, user.Role),   
                new Claim(ClaimTypes.MobilePhone, user.PhoneNumber)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}