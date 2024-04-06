using ShardingCore.Sharding.EntityQueryConfigurations;
using WebApplication1.Entities;

namespace WebApplication1.ShardingCore
{
    /// <summary>
    /// 优化分区表查询性能
    /// </summary>
    public class FileReferencesQueryConfiguration : IEntityQueryConfiguration<FileReference>
    {
        public void Configure(EntityQueryBuilder<FileReference> builder)
        {
            builder.ShardingTailComparer(Comparer<string>.Default, false);

            //设置分区表数据是按照时间升序分布，即：ORDER BY CreateTime ASC。
            builder.AddOrder(model => model.CreateTime, false, seqOrderMatch: SeqOrderMatchEnum.Named);

            //设置查询使用：FirstOrDefault时，按升序查询到数据时就放弃其余分区表查询。例如：从Q2查询到数据时，熔断Q3Q4的并发查询。false表示拉取升序最新一条数据，true表示降序最新一条数据。
            builder.AddDefaultSequenceQueryTrip(true, CircuitBreakerMethodNameEnum.FirstOrDefault);

            //配置熔断器，即顺序正向查找到数据时，熔断其他并发查询。
            builder.AddDefaultSequenceQueryTrip(true, CircuitBreakerMethodNameEnum.Enumerator, CircuitBreakerMethodNameEnum.FirstOrDefault);
        }
    }
}
