using CNPM.Api.Services.HubManage;
using CNPM.Api.Services.UserManage;

namespace CNPM.Api.Services
{
    public interface IServiceWrapper
    {
        IHubService HubManage { get; }
        IUserService UserManage { get; }
    }
}