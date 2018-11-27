using Microsoft.EntityFrameworkCore;
using PRZ.PushCenter.Subscriptions;

namespace PRZ.PushCenter
{
    public class PushCenterDbContext : DbContext
    {
        public PushCenterDbContext(DbContextOptions options) : base(options)
        {
        }

        public virtual DbSet<Subscription> Subscriptions { get; set; }
    }
}