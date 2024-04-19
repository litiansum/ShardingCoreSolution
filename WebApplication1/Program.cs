using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Serilog;
using ShardingCore;
using WebApplication1.EF;
using WebApplication1.ShardingCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();


var databaseConnectionString = builder.Configuration.GetConnectionString("ConnectionString");
//builder.Services.AddShardingDbContext<DemoDBContext>().UseRouteConfig(routeConfigure =>
//{
//    routeConfigure.AddShardingTableRoute<FileReferencesMonthVirtualTableRoute>();

//}).UseConfig(shardingConfigure =>
//{
//    //当查询无法匹配到对应的路由是否抛出异常 true表示抛出异常 false表示返回默认值
//    shardingConfigure.ThrowIfQueryRouteNotMatch = false;

//    shardingConfigure.MaxQueryConnectionsLimit = builder.Configuration.GetValue<int>("MaxQueryConnectionsLimit");

//    shardingConfigure.UseShardingQuery((connectionString, builder) => builder.UseSqlServer(connectionString));
//    shardingConfigure.UseShardingTransaction((connection, builder) => builder.UseSqlServer(connection));
//    shardingConfigure.UseShardingMigrationConfigure((configure) => configure.ReplaceService<IMigrationsSqlGenerator, ShardingSqlServerMigrationsSqlGenerator>());
//    shardingConfigure.AddDefaultDataSource(Guid.NewGuid().ToString("n"), databaseConnectionString);
//}).AddShardingCore();

builder.Services.AddDbContext<DemoDBContext>(options => options.UseNpgsql(databaseConnectionString));

//builder.Services.AddDbContext<DemoDBContext>(options => options.UseSqlServer(databaseConnectionString));

builder.Host.UseSerilog((hostingContext, services, loggerConfiguration) =>
{
    loggerConfiguration
     .ReadFrom.Configuration(hostingContext.Configuration)
     .WriteTo.Console();
});

var app = builder.Build();

InitializeDatabase(app);

// Configure the HTTP request pipeline.

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();



static void InitializeDatabase(IHost host)
{
    try
    {
        using var scope = host.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<DemoDBContext>();
        dbContext.Database.EnsureCreated();

        //运行数据迁移前，尝试创建分区表。
        scope.ServiceProvider.UseAutoTryCompensateTable();
    }
    catch (Exception ex)
    {
        Log.Error(ex, "Migrate Database Error");
    }
}