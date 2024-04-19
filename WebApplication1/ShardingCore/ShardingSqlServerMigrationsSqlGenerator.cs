using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore.Update;
using ShardingCore.Core.RuntimeContexts;
using ShardingCore.Helpers;
using System.Collections.Generic;
using System.Linq;

namespace WebApplication1.ShardingCore
{
    /// <summary>
    /// 重写SqlServer迁移
    /// </summary>
    public class ShardingSqlServerMigrationsSqlGenerator : SqlServerMigrationsSqlGenerator
    {
        /// <summary>
        /// ShardingCore迁移上下文
        /// </summary>
        private readonly IShardingRuntimeContext _shardingRuntimeContext;

        /// <summary>
        /// 功能：对象初始化器
        /// 作者：唐君军
        /// 时间：2023年11月24日12:02:47
        /// </summary>
        /// <param name="dependencies">MigrationsSqlGeneratorDependencies</param>
        /// <param name="migrationsAnnotations">IRelationalAnnotationProvider</param>
        /// <param name="shardingRuntimeContext">ShardingCore迁移上下文</param>
        public ShardingSqlServerMigrationsSqlGenerator(IShardingRuntimeContext shardingRuntimeContext,MigrationsSqlGeneratorDependencies dependencies, ICommandBatchPreparer commandBatchPreparer) : base(dependencies, commandBatchPreparer)
        {
            _shardingRuntimeContext = shardingRuntimeContext;
        }

        /// <summary>
        /// 功能：重写SqlServer迁移
        /// 作者：唐君军
        /// 时间：2023年11月24日12:02:07
        /// </summary>
        /// <param name="operation">MigrationOperation</param>
        /// <param name="model">IModel</param>
        /// <param name="builder">MigrationCommandListBuilder</param>
        protected override void Generate(MigrationOperation operation, IModel? model, MigrationCommandListBuilder builder)
        {
            List<MigrationCommand> oldCmds = builder.GetCommandList().ToList();
            base.Generate(operation, model, builder);
            List<MigrationCommand> newCmds = builder.GetCommandList().ToList();
            List<MigrationCommand> addCmds = newCmds.Where(x => !oldCmds.Contains(x)).ToList();
            MigrationHelper.Generate(_shardingRuntimeContext, operation, builder, Dependencies.SqlGenerationHelper, addCmds);
        }
    }
}
