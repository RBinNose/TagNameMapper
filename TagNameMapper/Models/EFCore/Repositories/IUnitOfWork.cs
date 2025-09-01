using System;
using System.Threading.Tasks;

namespace TagNameMapper.Models.EFCore.Repositories;

/// <summary>
/// 工作单元接口，用于管理多个 Repository 的事务
/// </summary>
public interface IUnitOfWork : IDisposable
{
    #region Repository 属性
    
    /// <summary>
    /// Tag Repository
    /// </summary>
    ITagRepository Tags { get; }
    
    /// <summary>
    /// TagGroup Repository
    /// </summary>
    ITagGroupRepository TagGroups { get; }
    
    #endregion
    
    #region 事务管理
    
    /// <summary>
    /// 保存所有更改
    /// </summary>
    /// <returns>受影响的行数</returns>
    Task<int> SaveChangesAsync();
    
    /// <summary>
    /// 开始事务
    /// </summary>
    /// <returns>事务对象</returns>
    Task BeginTransactionAsync();
    
    /// <summary>
    /// 提交事务
    /// </summary>
    Task CommitTransactionAsync();
    
    /// <summary>
    /// 回滚事务
    /// </summary>
    Task RollbackTransactionAsync();
    
    #endregion
}