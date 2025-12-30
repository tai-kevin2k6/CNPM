using CNPM.Api.Dtos;
using CNPM.DTOs;
using CNPM.Entities;
using static CNPM.Api.Dtos.Reponse;

namespace CNPM.Api.Services.HubManage
{
    public interface IHubService
    {
        Task<List<HubOptionDto>> GetHubOptionsForOwnerAsync(int userId);
        Task<PagedResult<OrderManagementDto>> GetOrdersForHubOwnerAsync(int userId, OrderFilterRequest request);
        Task<List<HubSummaryDto>> GetPendingHubsAsync();
        Task<HubDetailDto> GetHubDetailForAdminAsync(int hubId);
        Task<bool> ApproveHubAsync(ApproveHubRequest request);
        Task<List<HubSummaryDto>> GetAllHubsAsync(); // Xem danh sách
        Task<bool> UpdateOrderStatusAsync(UpdateOrderStatusRequest request);
        Task<bool> LockHubAsync(ApproveHubRequest request);
    }
}