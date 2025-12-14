using CNPM.Entities;
using Microsoft.EntityFrameworkCore;

namespace CNPM.Infrastructure.Data // Đã đổi tên namespace để tránh lỗi
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<PickupHub> PickupHubs { get; set; }
    }
}