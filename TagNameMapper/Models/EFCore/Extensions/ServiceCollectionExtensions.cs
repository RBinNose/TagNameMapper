using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TagNameMapper.Models.EFCore.DbContexts;
using TagNameMapper.Models.EFCore.Repositories;
using TagNameMapper.Models.TableMetadatas;

namespace TagNameMapper.Models.EFCore.Extensions;

/// <summary>
/// 服务集合扩展方法，用于注册数据访问层服务
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// 注册数据访问层服务
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <param name="connectionString">数据库连接字符串</param>
    /// <returns>服务集合</returns>
    public static IServiceCollection AddDataAccessServices(this IServiceCollection services, string connectionString)
    {
        // 注册 DbContext
        services.AddDbContext<TagNameMapperDbContext>(options =>
        {
            options.UseSqlite(connectionString);
            options.EnableSensitiveDataLogging(); // 开发环境启用敏感数据日志
            options.EnableDetailedErrors(); // 启用详细错误信息
        });

        // 注册 Repository 接口和实现
        services.AddScoped<IRepository<Tag>, Repository<Tag>>();
        services.AddScoped<IRepository<TagGroup>, Repository<TagGroup>>();
        
        services.AddScoped<ITagRepository, TagRepository>();
        services.AddScoped<ITagGroupRepository, TagGroupRepository>();

        // 注册工作单元
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }

    /// <summary>
    /// 注册数据访问层服务（使用默认连接字符串）
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <returns>服务集合</returns>
    public static IServiceCollection AddDataAccessServices(this IServiceCollection services)
    {
        var databasePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"Data", "DataBase.sqlite");
        var defaultConnectionString = $"Data Source={Path.GetFullPath(databasePath)}";
        return AddDataAccessServices(services, defaultConnectionString);
    }

    /// <summary>
    /// 确保数据库已创建
    /// </summary>
    /// <param name="serviceProvider">服务提供者</param>
    /// <returns>异步任务</returns>
    public static async Task EnsureDatabaseCreatedAsync(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<TagNameMapperDbContext>();
        
        // 确保数据库已创建
        await context.Database.EnsureCreatedAsync();
        
        // 可以在这里添加种子数据
        await SeedDataAsync(context);
    }

    /// <summary>
    /// 种子数据初始化
    /// </summary>
    /// <param name="context">数据库上下文</param>
    /// <returns>异步任务</returns>
    private static async Task SeedDataAsync(TagNameMapperDbContext context)
    {
        // 检查是否已有数据
        if (await context.TagGroups.AnyAsync())
            return;

        // 创建默认的根组
        var rootGroup = new TagGroup
        {
            Name = "Root",
            GroupType = Enums.TagGroupType.RootFolder,
            ParentGroupId = null,
        };

        context.TagGroups.Add(rootGroup);

        // 创建一些示例组
        var plcGroup = new TagGroup
        {
            Name = "PLC变量",
            GroupType = Enums.TagGroupType.Folder,
            ParentGroupId = rootGroup.Id,
        };

        var hmiGroup = new TagGroup
        {
            Name = "HMI变量",
            GroupType = Enums.TagGroupType.Folder,
            ParentGroupId =  rootGroup.Id,
        };

        var plctest1 = new TagGroup
        {
            Name = "plctest1",
            GroupType = Enums.TagGroupType.Folder,
            ParentGroupId = plcGroup.Id,
        };

        var plctest1v1 = new TagGroup
        {
            Name = "plctest1v1",
            GroupType = Enums.TagGroupType.TagTable,
            ParentGroupId = plctest1.Id,
        };

        context.TagGroups.AddRange(plcGroup, hmiGroup, plctest1, plctest1v1);

        await context.SaveChangesAsync();
    }
}