using CNPM.Entities;
using CNPM.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CNPM.Api.Services
{
    public class PickupHubRepository : IPickupHubRepository
    {
        private readonly ApplicationDbContext _context; // Đổi tên ApplicationDbContext theo tên DbContext của bạn

        public PickupHubRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PickupHub> GetHubDetailByIdAsync(int id)
        {
            // Quan trọng: Phải có .Include để lấy thông tin bảng Owner
            var hub = await _context.PickupHubs
                .Include(h => h.Owner)
                .FirstOrDefaultAsync(h => h.Id == id);

            return hub;
        }
    }
}
