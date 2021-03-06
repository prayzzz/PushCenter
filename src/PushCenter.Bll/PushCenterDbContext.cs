using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using PushCenter.Bll.Subscriptions;

namespace PushCenter.Bll
{
    [UsedImplicitly]
    public sealed class PushCenterDbContext : DbContext
    {
        public PushCenterDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Subscription> Subscriptions { get; set; }
    }
}
