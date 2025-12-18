using CNPM.Entities;
using CNPM.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CNPM.Api.Services
{
    public class PickupHubRepository : IPickupHubRepository
    {
        private readonly ApplicationDbContext _context;

        public PickupHubRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PickupHub> GetHubDetailByIdAsync(int id)
        {
            var hub = await _context.PickupHubs
                .FirstOrDefaultAsync(h => h.Id == id);

            return hub;
        }
    }
}
