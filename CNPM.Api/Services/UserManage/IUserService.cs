using CNPM.DTOs;
using CNPM.Entities;

namespace CNPM.Api.Services.UserManage
{
    public interface IUserService
    {
        Task<List<UserSummaryDto>> GetAllUsersAsync(); // Xem danh sách
        Task<bool> ApproveUser(ApproveUserRequest request); // Khóa/Mở
    }
}