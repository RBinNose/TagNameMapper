using System.Collections.Generic;
using System.Threading.Tasks;
using TagNameMapper.Models.TableMetadatas;
using TagNameMapper.Enums;
using System;

namespace TagNameMapper.Models.EFCore.Repositories;

/// <summary>
/// Tag 专用 Repository 接口，继承通用 Repository 并提供 Tag 特定的操作
/// </summary>
public interface ITagRepository : IRepository<Tag>
{
    #region Tag 特定查询
    
    /// <summary>
    /// 根据名称获取 Tag
    /// </summary>
    /// <param name="name">Tag 名称</param>
    /// <returns>Tag 对象，如果不存在则返回 null</returns>
    Task<Tag?> GetByNameAsync(string name);
    
    /// <summary>
    /// 获取指定组下的所有 Tag
    /// </summary>
    /// <param name="groupId">组 ID</param>
    /// <returns>该组下的所有 Tag</returns>
    Task<IEnumerable<Tag>> GetByGroupIdAsync(Guid groupId);
    
    
    
    
    /// <summary>
    /// 搜索 Tag（根据名称、描述、地址等字段模糊匹配）
    /// </summary>
    /// <param name="keyword">搜索关键词</param>
    /// <returns>匹配的 Tag 集合</returns>
    Task<IEnumerable<Tag>> SearchAsync(string keyword);
    
    #endregion
    
    #region Tag 特定操作
    
    /// <summary>
    /// 将 Tag 移动到指定TagTable
    /// </summary>
    /// <param name="tagId">Tag ID</param>
    /// <param name="newTagTableId">新TagTable ID，组类型必须是TagTable</param>
    /// <returns>是否移动成功</returns>
    Task<bool> MoveToTagTableAsync(Guid tagId, Guid? newTagTableId);
    
    /// <summary>
    /// 批量将 Tag 移动到指定TagTable
    /// </summary>
    /// <param name="tagIds">Tag ID 集合</param>
    /// <param name="newTagTableId">新TagTable ID，组类型必须是TagTable</param>
    /// <returns>成功移动的 Tag 数量</returns>
    Task<int> MoveTagsToTagTableAsync(IEnumerable<Guid> tagIds, Guid? newTagTableId);
    
    /// <summary>
    /// 复制 Tag 到指定组，新Tag名称为“Copy of [原名称+_Count]”
    /// </summary>
    /// <param name="sourceTagId">源 Tag ID</param>
    /// <param name="targetTagTableId">目标组 ID</param>
    /// <returns>复制后的新 Tag</returns>
    Task<Tag?> CopyToTagTableAsync(Guid sourceTagId, Guid? targetTagTableId);
    
    /// <summary>
    /// 检查 Tag 名称是否已存在
    /// </summary>
    /// <param name="name">Tag 名称</param>
    /// <param name="excludeId">排除的 Tag ID（用于更新时检查）</param>
    /// <returns>是否已存在</returns>
    Task<bool> IsNameExistsAsync(string name, Guid? excludeId = null);
    
    /// <summary>
    /// 获取 Tag 的完整信息（包含关联的组信息）
    /// </summary>
    /// <param name="id">Tag ID</param>
    /// <returns>包含组信息的 Tag</returns>
    Task<Tag?> GetWithGroupAsync(Guid id);
    
    /// <summary>
    /// 批量获取 Tag 的完整信息（包含关联的组信息）
    /// </summary>
    /// <param name="ids">Tag ID 集合</param>
    /// <returns>包含组信息的 Tag 集合</returns>
    Task<IEnumerable<Tag>> GetWithGroupAsync(IEnumerable<Guid> ids);
    
    #endregion
}