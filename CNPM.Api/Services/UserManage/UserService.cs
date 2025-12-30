using CNPM.Api.Dtos;
using CNPM.DTOs;
using CNPM.Entities;
using CNPM.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static CNPM.Api.Dtos.Reponse; // Lưu ý sửa typo Reponse -> Response nếu cần

namespace CNPM.Api.Services.UserManage
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;
        public UserService(ApplicationDbContext context) { _context = context; }

        public async Task<List<UserSummaryDto>> GetAllUsersAsync()
        {
            return await _context.Users
                .AsNoTracking()
                .Where(u => u.Role != "admin")
                .Select(u => new UserSummaryDto
                {
                    Id = u.Id,
                    FullName = u.FullName,
                    PhoneNumber = u.PhoneNumber,
                    Status = u.Status,
                    Role = u.Role
                })
                .ToListAsync();
        }

        public async Task<bool> ApproveUser(ApproveUserRequest request)
        {
            var user = await _context.Users.FindAsync(request.UserId);
            if (user == null) throw new Exception("Không tìm thấy User.");

            // Cập nhật trạng thái
            if (request.IsLocked == true)
            {
                user.Status = "locked";
                user.Reason = request.Reason;
            }
            else
            {
                user.Status = "active";
                user.Reason = null;
            }


                _context.Users.Update(user);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}