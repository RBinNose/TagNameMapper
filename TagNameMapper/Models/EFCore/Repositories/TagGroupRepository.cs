using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TagNameMapper.Models.EFCore.DbContexts;
using TagNameMapper.Models.TableMetadatas;
using TagNameMapper.Enums;
using System.Collections.ObjectModel;

namespace TagNameMapper.Models.EFCore.Repositories;

/// <summary>
/// TagGroup Repository 具体实现
/// </summary>
public class TagGroupRepository : Repository<TagGroup>, ITagGroupRepository
{
    public TagGroupRepository(TagNameMapperDbContext context) : base(context)
    {
    }

    #region TagGroup 特定查询

    public async Task<TagGroup?> GetByNameAsync(string name)
    {
        return await _dbSet.FirstOrDefaultAsync(g => g.Name == name);
    }

    public async Task<IEnumerable<TagGroup>> GetRootGroupsAsync()
    {
        var allGroups = await _dbSet.ToListAsync();
        var groupLookup = allGroups.ToLookup(g => g.ParentGroupId);

        foreach (var group in allGroups)
        {
           group.ChildGroups = new ObservableCollection<TagGroup>(groupLookup[group.Id].OrderBy(c => c.Name));
        }

        return groupLookup[null].OrderBy(g => g.Name).ToList();
    }

    public async Task<IEnumerable<TagGroup>> GetChildGroupsAsync(Guid parentId)
    {
        return await _dbSet
            .Where(g => g.ParentGroupId == parentId)
            .OrderBy(g => g.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<TagGroup>> GetDescendantGroupsAsync(Guid parentId)
    {
        var descendants = new List<TagGroup>();
        var queue = new Queue<Guid>();
        queue.Enqueue(parentId);

        while (queue.Count > 0)
        {
            var currentParentId = queue.Dequeue();
            var children = await _dbSet
                .Where(g => g.ParentGroupId == currentParentId)
                .ToListAsync();

            descendants.AddRange(children);
            foreach (var child in children)
            {
                queue.Enqueue(child.Id);
            }
        }

        return descendants.OrderBy(g => g.Name);
    }

    public async Task<IEnumerable<TagGroup>> GetAncestorGroupsAsync(Guid groupId)
    {
        var ancestors = new List<TagGroup>();
        var currentGroup = await _dbSet.FindAsync(groupId);

        while (currentGroup?.ParentGroupId != null)
        {
            var parent = await _dbSet.FindAsync(currentGroup.ParentGroupId.Value);
            if (parent != null)
            {
                ancestors.Insert(0, parent); // 插入到开头，保持从根到父级的顺序
                currentGroup = parent;
            }
            else
            {
                break;
            }
        }

        return ancestors;
    }

    public async Task<IEnumerable<TagGroup>> GetByTypeAsync(TagGroupType groupType)
    {
        return await _dbSet
            .Where(g => g.GroupType == groupType)
            .OrderBy(g => g.Name)
            .ToListAsync();
    }


    public async Task<IEnumerable<TagGroup>> GetFullTreeAsync()
    {
        return await _dbSet
            .Include(g => g.ChildGroups)
            .Include(g => g.Tags)
            .OrderBy(g => g.Name)
            .ToListAsync();
    }

    #endregion

    #region TagGroup 特定操作

    public async Task<bool> MoveToParentAsync(Guid groupId, Guid? newParentId)
    {
        var group = await _dbSet.FindAsync(groupId);
        if (group == null)
            return false;

        // 检查是否会造成循环引用
        if (newParentId.HasValue && !await CanMoveToParentAsync(groupId, newParentId))
            return false;

        // 如果新父组ID不为null，验证父组是否存在
        if (newParentId.HasValue)
        {
            var parentExists = await _dbSet.AnyAsync(g => g.Id == newParentId.Value);
            if (!parentExists)
                return false;
        }

        group.ParentGroupId = newParentId;
        return true;
    }

    public async Task<TagGroup?> CopyToParentAsync(Guid sourceGroupId, Guid? targetParentId, string? newName = null, 
        bool includeChildren = true, bool includeTags = true)
    {
        var sourceGroup = await _dbSet
            .Include(g => g.ChildGroups)
            .Include(g => g.Tags)
            .FirstOrDefaultAsync(g => g.Id == sourceGroupId);

        if (sourceGroup == null)
            return null;

        // 如果目标父组ID不为null，验证父组是否存在
        if (targetParentId.HasValue)
        {
            var parentExists = await _dbSet.AnyAsync(g => g.Id == targetParentId.Value);
            if (!parentExists)
                return null;
        }

        // 生成新名称
        if (string.IsNullOrEmpty(newName))
        {
            var baseName = sourceGroup.Name;
            var counter = 1;
            do
            {
                newName = $"{baseName}_Copy{counter}";
                counter++;
            } while (await IsNameExistsInParentAsync(newName, targetParentId));
        }

        // 创建新组
        var newGroup = new TagGroup
        {
            Name = newName,
            GroupType = sourceGroup.GroupType,
            ParentGroupId = targetParentId
        };

        await _dbSet.AddAsync(newGroup);
        await _context.SaveChangesAsync(); // 保存以获取新组的ID

        // 复制标签
        if (includeTags && sourceGroup.Tags.Any())
        {
            foreach (var tag in sourceGroup.Tags)
            {
                var newTag = new Tag
                {
                    Name = tag.Name,
                    Description = tag.Description,
                    DataType = tag.DataType,
                    Address = tag.Address,
                    TagGroupId = newGroup.Id,
                };
                await _context.Tags.AddAsync(newTag);
            }
        }

        // 递归复制子组
        if (includeChildren && sourceGroup.ChildGroups.Any())
        {
            foreach (var childGroup in sourceGroup.ChildGroups)
            {
                await CopyToParentAsync(childGroup.Id, newGroup.Id, null, true, includeTags);
            }
        }

        return newGroup;
    }

    public async Task<bool> IsNameExistsInParentAsync(string name, Guid? parentId, Guid? excludeId = null)
    {
        var query = _dbSet.Where(g => g.Name == name && g.ParentGroupId == parentId);
        if (excludeId.HasValue)
        {
            query = query.Where(g => g.Id != excludeId.Value);
        }
        return await query.AnyAsync();
    }

    public async Task<bool> CanMoveToParentAsync(Guid groupId, Guid? targetParentId)
    {
        if (!targetParentId.HasValue)
            return true; // 移动到根级别总是可以的

        // 检查目标父组是否是当前组的后代（避免循环引用）
        var descendants = await GetDescendantGroupsAsync(groupId);
        return !descendants.Any(d => d.Id == targetParentId.Value);
    }

    public async Task<TagGroup?> GetWithFullInfoAsync(Guid id)
    {
        return await _dbSet
            .Include(g => g.ParentGroup)
            .Include(g => g.ChildGroups)
            .Include(g => g.Tags)
            .FirstOrDefaultAsync(g => g.Id == id);
    }

    public async Task<int> GetTotalTagCountAsync(Guid groupId)
    {
        var group = await _dbSet
            .Include(g => g.Tags)
            .FirstOrDefaultAsync(g => g.Id == groupId);

        if (group == null)
            return 0;

        var directTagCount = group.Tags.Count;
        var descendants = await GetDescendantGroupsAsync(groupId);
        var descendantTagCount = 0;

        foreach (var descendant in descendants)
        {
            var descendantGroup = await _dbSet
                .Include(g => g.Tags)
                .FirstOrDefaultAsync(g => g.Id == descendant.Id);
            if (descendantGroup != null)
            {
                descendantTagCount += descendantGroup.Tags.Count;
            }
        }

        return directTagCount + descendantTagCount;
    }

    public async Task<bool> DeleteWithChildrenAsync(Guid groupId, bool deleteChildGroups = true, bool deleteTags = false)
    {
        var group = await GetWithFullInfoAsync(groupId);
        if (group == null)
            return false;

        // 删除或移动标签
        if (group.Tags.Any())
        {
            if (deleteTags)
            {
                _context.Tags.RemoveRange(group.Tags);
            }
            else
            {
                // 将标签移动到父组或根级别
                foreach (var tag in group.Tags)
                {
                    tag.TagGroupId = group.ParentGroupId;
                }
            }
        }

        // 删除或移动子组
        if (group.ChildGroups.Any())
        {
            if (deleteChildGroups)
            {
                foreach (var childGroup in group.ChildGroups)
                {
                    await DeleteWithChildrenAsync(childGroup.Id, true, deleteTags);
                }
            }
            else
            {
                // 将子组移动到父组或根级别
                foreach (var childGroup in group.ChildGroups)
                {
                    childGroup.ParentGroupId = group.ParentGroupId;
                }
            }
        }

        _dbSet.Remove(group);
        return true;
    }


    #endregion

}
