using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure.Internal;
using Npgsql.EntityFrameworkCore.PostgreSQL.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Migrations.Operations;
using ShardingCore.Core.RuntimeContexts;
using ShardingCore.Helpers;
using System.Collections.Generic;
using System.Linq;

namespace WebApplication1.ShardingCore
{
    /// <summary>
    /// Sharding数据迁移管理
    /// </summary>
    public class ShardingPostgreSqlMigrationsSqlGenerator : NpgsqlMigrationsSqlGenerator
    {
        /// <summary>
        /// 迁移上下文
        /// </summary>
        private readonly IShardingRuntimeContext _shardingRuntimeContext;

        /// <summary>
        /// 功能：初始化Sharding数据迁移管理
        /// 作者：唐君军
        /// 时间：2023年11月21日14:36:22
        /// </summary>
        /// <param name="dependencies">MigrationsSqlGeneratorDependencies</param>
        /// <param name="migrationsAnnotations">INpgsqlSingletonOptions</param>
        /// <param name="shardingRuntimeContext">IShardingRuntimeContext</param>
        public ShardingPostgreSqlMigrationsSqlGenerator(MigrationsSqlGeneratorDependencies dependencies, INpgsqlSingletonOptions migrationsAnnotations, IShardingRuntimeContext shardingRuntimeContext) :
            base(dependencies, migrationsAnnotations)
        {
            _shardingRuntimeContext = shardingRuntimeContext;
        }

        /// <summary>
        /// 功能：重写Sql迁移
        /// 作者：唐君军
        /// 时间：2023年11月21日14:38:06
        /// </summary>
        /// <param name="operation">NpgsqlDropDatabaseOperation</param>
        /// <param name="model">IModel</param>
        /// <param name="builder">MigrationCommandListBuilder</param>
        public override void Generate(NpgsqlDropDatabaseOperation operation, IModel? model, MigrationCommandListBuilder builder)
        {
            List<MigrationCommand> oldCmds = builder.GetCommandList().ToList();
            List<MigrationCommand> newCmds = builder.GetCommandList().ToList();
            List<MigrationCommand> addCmds = newCmds.Where(x => !oldCmds.Contains(x)).ToList();
            MigrationHelper.Generate(_shardingRuntimeContext, operation, builder, Dependencies.SqlGenerationHelper, addCmds);
        }
    }
}
