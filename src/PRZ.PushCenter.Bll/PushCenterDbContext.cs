using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using PRZ.PushCenter.Bll.Subscriptions;

namespace PRZ.PushCenter.Bll
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