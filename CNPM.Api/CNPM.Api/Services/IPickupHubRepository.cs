using CNPM.Entities;

namespace CNPM.Api.Services
{
    public interface IPickupHubRepository
    {
        // Hàm lấy Hub theo Id, kèm theo thông tin Owner
        Task<PickupHub> GetHubDetailByIdAsync(int id);
    }
}
