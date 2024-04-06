using Microsoft.EntityFrameworkCore;
using ShardingCore.Core.EntityMetadatas;
using ShardingCore.Core.VirtualRoutes;
using ShardingCore.Sharding.EntityQueryConfigurations;
using ShardingCore.Sharding.PaginationConfigurations;
using ShardingCore.VirtualRoutes.Months;
using WebApplication1.Entities;

namespace WebApplication1.ShardingCore
{
    public class FileReferencesMonthVirtualTableRoute : AbstractSimpleShardingMonthKeyLongVirtualTableRoute<FileReference>
    {
        public override bool AutoCreateTableByTime()
        {
            return true;
        }

        public override DateTime GetBeginTime()
        {
            return new DateTime(2022, 1, 1, 0, 0, 0, kind: DateTimeKind.Local);
        }


        public override void Configure(EntityMetadataTableBuilder<FileReference> builder)
        {
            builder.ShardingProperty(t => t.CreateTime);
            builder.ShardingExtraProperty(t => t.ReferenceId);
        }


        /// <returns>IEntityQueryConfiguration</returns>
        public override IEntityQueryConfiguration<FileReference> CreateEntityQueryConfiguration()
        {
            return new FileReferencesQueryConfiguration();
        }

        /// <returns>IPaginationConfiguration</returns>
        public override IPaginationConfiguration<FileReference> CreatePaginationConfiguration()
        {
            return new FileReferencesPaginationConfiguration();
        }

        /// <summary>
        /// 功能：设置多字段分片路由
        /// </summary>
        /// <param name="shardingKey">分片字段值</param>
        /// <param name="shardingOperator">分片操作</param>
        /// <param name="shardingPropertyName">分片字段名称</param>
        /// <returns>Func</returns>
        public override Func<string, bool> GetExtraRouteFilter(object shardingKey, ShardingOperatorEnum shardingOperator, string shardingPropertyName)
        {
            if (shardingPropertyName == nameof(FileReference.ReferenceId))
            {
                string referenceId = shardingKey?.ToString() ?? string.Empty;
                if (string.IsNullOrEmpty(referenceId))
                    return tail => true;

                //1.当ReferenceId转换long失败时，表示旧数据 旧的历史记录，即原本的Guid值，此时只能对所有分区表进行全表扫描。
                if (!long.TryParse(referenceId, out long snowflakeId) || snowflakeId <= 0)
                    return tail => true;

                //2.当转换成功时，表示采用雪花算法生成的Id，此时按照雪花算法规则解析出时间。
                var time = SnowflakeIdHelper.GetDateTime(snowflakeId);

                if (time.LocalDateTime > DateTime.Now) return tail => true;

                long currentDatetime = time.ToUnixTimeMilliseconds();

                //3.当得到时间后就能根据时间进行路由匹配，更加准确的将查询锁定在指定分区表中，提高整体查询效率。
                return GetRouteToFilter(currentDatetime, shardingOperator);
            }
            return base.GetExtraRouteFilter(shardingKey, shardingOperator, shardingPropertyName);
        }
    }
}