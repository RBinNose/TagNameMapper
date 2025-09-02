using System.Collections.Generic;
using System.Threading.Tasks;
using TagNameMapper.Models.TableMetadatas;
using TagNameMapper.Enums;
using System;

namespace TagNameMapper.Models.EFCore.Repositories;

/// <summary>
/// TagGroup 专用 Repository 接口，继承通用 Repository 并提供 TagGroup 特定的操作
/// </summary>
public interface ITagGroupRepository : IRepository<TagGroup>
{
    #region TagGroup 特定查询
    
    /// <summary>
    /// 根据名称获取 TagGroup
    /// </summary>
    /// <param name="name">TagGroup 名称</param>
    /// <returns>TagGroup 对象，如果不存在则返回 null</returns>
    Task<TagGroup?> GetByNameAsync(string name);
    
    /// <summary>
    /// 获取根级别的 TagGroup（ParentGroupId 为 null）
    /// </summary>
    /// <returns>根级别的 TagGroup 集合</returns>
    Task<IEnumerable<TagGroup>> GetRootGroupsAsync();
    
    /// <summary>
    /// 获取指定父组下的子组
    /// </summary>
    /// <param name="parentId">父组 ID</param>
    /// <returns>子组集合</returns>
    Task<IEnumerable<TagGroup>> GetChildGroupsAsync(Guid parentId);
    
    /// <summary>
    /// 获取指定组的所有后代组（递归获取）
    /// </summary>
    /// <param name="parentId">父组 ID</param>
    /// <returns>所有后代组集合</returns>
    Task<IEnumerable<TagGroup>> GetDescendantGroupsAsync(Guid parentId);
    
    /// <summary>
    /// 获取指定组的所有祖先组（从根到父级的路径）
    /// </summary>
    /// <param name="groupId">组 ID</param>
    /// <returns>祖先组集合（按层级排序）</returns>
    Task<IEnumerable<TagGroup>> GetAncestorGroupsAsync(Guid groupId);
    
    /// <summary>
    /// 根据组类型获取 TagGroup
    /// </summary>
    /// <param name="groupType">组类型</param>
    /// <returns>指定类型的 TagGroup 集合</returns>
    Task<IEnumerable<TagGroup>> GetByTypeAsync(TagGroupType groupType);
    
    
    
    /// <summary>
    /// 获取完整的树形结构（包含所有子组和标签）
    /// </summary>
    /// <returns>完整的树形结构</returns>
    Task<IEnumerable<TagGroup>> GetFullTreeAsync();
    
    #endregion
    
    #region TagGroup 特定操作
    
    /// <summary>
    /// 将组移动到新的父组下
    /// </summary>
    /// <param name="groupId">要移动的组 ID</param>
    /// <param name="newParentId">新父组 ID，null 表示移动到根级别</param>
    /// <returns>是否移动成功</returns>
    Task<bool> MoveToParentAsync(Guid groupId, Guid? newParentId);
    
    /// <summary>
    /// 复制组到指定父组下（包括所有子组和标签）
    /// </summary>
    /// <param name="sourceGroupId">源组 ID</param>
    /// <param name="targetParentId">目标父组 ID</param>
    /// <param name="newName">新组名称（如果为空则自动生成）</param>
    /// <param name="includeChildren">是否包含子组</param>
    /// <param name="includeTags">是否包含标签</param>
    /// <returns>复制后的新组</returns>
    Task<TagGroup?> CopyToParentAsync(Guid sourceGroupId, Guid? targetParentId, string? newName = null, 
        bool includeChildren = true, bool includeTags = true);
    
    /// <summary>
    /// 检查组名称在指定父组下是否已存在
    /// </summary>
    /// <param name="name">组名称</param>
    /// <param name="parentId">父组 ID</param>
    /// <param name="excludeId">排除的组 ID（用于更新时检查）</param>
    /// <returns>是否已存在</returns>
    Task<bool> IsNameExistsInParentAsync(string name, Guid? parentId, Guid? excludeId = null);
    
    /// <summary>
    /// 检查是否可以移动组（避免循环引用）
    /// </summary>
    /// <param name="groupId">要移动的组 ID</param>
    /// <param name="targetParentId">目标父组 ID</param>
    /// <returns>是否可以移动</returns>
    Task<bool> CanMoveToParentAsync(Guid groupId, Guid? targetParentId);
    
    /// <summary>
    /// 获取组的完整信息（包含父组、子组和标签）
    /// </summary>
    /// <param name="id">组 ID</param>
    /// <returns>包含完整信息的组</returns>
    Task<TagGroup?> GetWithFullInfoAsync(Guid id);
    
    /// <summary>
    /// 获取组及其所有子组的标签数量统计
    /// </summary>
    /// <param name="groupId">组 ID</param>
    /// <returns>标签数量（包含子组中的标签）</returns>
    Task<int> GetTotalTagCountAsync(Guid groupId);
    
    /// <summary>
    /// 删除组及其所有子组和标签
    /// </summary>
    /// <param name="groupId">组 ID</param>
    /// <param name="deleteChildGroups">是否删除子组</param>
    /// <param name="deleteTags">是否删除标签</param>
    /// <returns>是否删除成功</returns>
    Task<bool> DeleteWithChildrenAsync(Guid groupId, bool deleteChildGroups = true, bool deleteTags = false);
    
   
    
    #endregion
}