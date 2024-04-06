using ShardingCore.Sharding.PaginationConfigurations;
using WebApplication1.Entities;

namespace WebApplication1.ShardingCore
{
    /// <summary>
    /// 开启分区表高性能分页查询
    /// </summary>
    public class FileReferencesPaginationConfiguration : IPaginationConfiguration<FileReference>
    {
        public void Configure(PaginationBuilder<FileReference> builder)
        {
            //设置分区表数据是按时间降序分布 路由表是正序分布
            builder.PaginationSequence(o => o.CreateTime)
                .UseRouteComparerAsc(Comparer<string>.Default)
                .UseQueryMatch(PaginationMatchEnum.Owner | PaginationMatchEnum.Named | PaginationMatchEnum.PrimaryMatch)
                .UseAppendIfOrderNone();
            builder.ConfigReverseShardingPage(0.5d, 10000L);
        }
    }
}
