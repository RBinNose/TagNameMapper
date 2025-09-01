using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace TagNameMapper.Models.EFCore.Repositories;

/// <summary>
/// 通用 Repository 接口，提供基本的 CRUD 操作
/// </summary>
/// <typeparam name="T">实体类型</typeparam>
public interface IRepository<T> where T : class
{
    #region 查询操作
    
    /// <summary>
    /// 根据 ID 获取实体
    /// </summary>
    /// <param name="id">实体 ID</param>
    /// <returns>实体对象，如果不存在则返回 null</returns>
    Task<T?> GetByIdAsync(int id);
    
    /// <summary>
    /// 获取所有实体
    /// </summary>
    /// <returns>所有实体的集合</returns>
    Task<IEnumerable<T>> GetAllAsync();
    
    /// <summary>
    /// 根据条件查找实体
    /// </summary>
    /// <param name="predicate">查询条件</param>
    /// <returns>符合条件的实体集合</returns>
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
    
    /// <summary>
    /// 根据条件获取第一个实体
    /// </summary>
    /// <param name="predicate">查询条件</param>
    /// <returns>第一个符合条件的实体，如果不存在则返回 null</returns>
    Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);
    
    /// <summary>
    /// 分页查询
    /// </summary>
    /// <param name="pageIndex">页索引（从 0 开始）</param>
    /// <param name="pageSize">页大小</param>
    /// <returns>分页结果和总数量</returns>
    Task<(IEnumerable<T> Items, int TotalCount)> GetPagedAsync(int pageIndex, int pageSize);
    
    /// <summary>
    /// 根据条件分页查询
    /// </summary>
    /// <param name="predicate">查询条件</param>
    /// <param name="pageIndex">页索引（从 0 开始）</param>
    /// <param name="pageSize">页大小</param>
    /// <returns>分页结果和总数量</returns>
    Task<(IEnumerable<T> Items, int TotalCount)> GetPagedAsync(
        Expression<Func<T, bool>> predicate, 
        int pageIndex, 
        int pageSize);
    
    /// <summary>
    /// 检查是否存在符合条件的实体
    /// </summary>
    /// <param name="predicate">查询条件</param>
    /// <returns>是否存在</returns>
    Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);
    
    /// <summary>
    /// 获取符合条件的实体数量
    /// </summary>
    /// <param name="predicate">查询条件</param>
    /// <returns>实体数量</returns>
    Task<int> CountAsync(Expression<Func<T, bool>> predicate);
    
    #endregion
    
    #region 修改操作
    
    /// <summary>
    /// 添加实体
    /// </summary>
    /// <param name="entity">要添加的实体</param>
    /// <returns>添加后的实体</returns>
    Task<T> AddAsync(T entity);
    
    /// <summary>
    /// 批量添加实体
    /// </summary>
    /// <param name="entities">要添加的实体集合</param>
    /// <returns>添加的实体数量</returns>
    Task<int> AddRangeAsync(IEnumerable<T> entities);
    
    /// <summary>
    /// 更新实体
    /// </summary>
    /// <param name="entity">要更新的实体</param>
    /// <returns>更新后的实体</returns>
    Task<T> UpdateAsync(T entity);
    
    /// <summary>
    /// 批量更新实体
    /// </summary>
    /// <param name="entities">要更新的实体集合</param>
    /// <returns>更新的实体数量</returns>
    Task<int> UpdateRangeAsync(IEnumerable<T> entities);
    
    /// <summary>
    /// 删除实体
    /// </summary>
    /// <param name="id">要删除的实体 ID</param>
    /// <returns>是否删除成功</returns>
    Task<bool> DeleteAsync(int id);
    
    /// <summary>
    /// 删除实体
    /// </summary>
    /// <param name="entity">要删除的实体</param>
    /// <returns>是否删除成功</returns>
    Task<bool> DeleteAsync(T entity);
    
    /// <summary>
    /// 批量删除实体
    /// </summary>
    /// <param name="entities">要删除的实体集合</param>
    /// <returns>删除的实体数量</returns>
    Task<int> DeleteRangeAsync(IEnumerable<T> entities);
    
    /// <summary>
    /// 根据条件删除实体
    /// </summary>
    /// <param name="predicate">删除条件</param>
    /// <returns>删除的实体数量</returns>
    Task<int> DeleteWhereAsync(Expression<Func<T, bool>> predicate);
    
    #endregion
    
    #region 工作单元
    
    /// <summary>
    /// 保存所有更改
    /// </summary>
    /// <returns>受影响的行数</returns>
    Task<int> SaveChangesAsync();
    
    #endregion
}