using CNPM.Api.Services.HubManage;
using CNPM.Api.Services.UserManage;
using CNPM.Infrastructure.Data;

namespace CNPM.Api.Services
{
    public class ServiceWrapper : IServiceWrapper
    {
        private readonly ApplicationDbContext _context;

        // SỬA 1: Khai báo đúng Interface riêng biệt cho từng biến
        private IHubService _hubManage;  // (Lưu ý: Check lại tên Interface file HubManage là IHubService hay IHubManageService)
        private IUserService _userManage;

        public ServiceWrapper(ApplicationDbContext context)
        {
            _context = context;
        }
        
        public IHubService HubManage
        {
            get
            {
                if (_hubManage == null)
                {
                    _hubManage = new HubService(_context);
                }
                return _hubManage;
            }
        }

        public IUserService UserManage
        {
            get
            {
                if (_userManage == null)
                {
                    _userManage = new UserService(_context);
                }
                return _userManage;
            }
        }
    }
}