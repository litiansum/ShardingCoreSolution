using Microsoft.EntityFrameworkCore;
using ShardingCore.Sharding.Abstractions;
using ShardingCore.Sharding;
using WebApplication1.Entities;
using ShardingCore.Core.VirtualRoutes.TableRoutes.RouteTails.Abstractions;

namespace WebApplication1.EF
{
    public class DemoDBContext : AbstractShardingDbContext, IShardingTableDbContext
    {
        public DemoDBContext(DbContextOptions<DemoDBContext> options) : base(options)
        {

        }

        public IRouteTail RouteTail { get; set; } = null!;

        public DbSet<Entities.FileInfo> FileInfos { get; set; } = null!;

        public DbSet<FileReference> FileReferences { get; set; } = null!;
    }
}
