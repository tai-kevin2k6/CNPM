using CNPM.Entities;

namespace CNPM.Api.Services
{
    public interface IPickupHubRepository
    {        Task<PickupHub> GetHubDetailByIdAsync(int id);
    }
}
