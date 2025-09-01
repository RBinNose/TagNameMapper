using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TagNameMapper.Models.EFCore.DbContexts;
using TagNameMapper.Models.TableMetadatas;
using TagNameMapper.Enums;

namespace TagNameMapper.Models.EFCore.Repositories;

/// <summary>
/// Tag Repository 具体实现
/// </summary>
public class TagRepository : Repository<Tag>, ITagRepository
{
    public TagRepository(TagNameMapperDbContext context) : base(context)
    {
    }

    #region Tag 特定查询

    public async Task<Tag?> GetByNameAsync(string name)
    {
        return await _dbSet.FirstOrDefaultAsync(t => t.Name == name);
    }

    public async Task<IEnumerable<Tag>> GetByGroupIdAsync(int groupId)
    {
        return await _dbSet
            .Where(t => t.TagGroupId == groupId)
            .OrderBy(t => t.Name)
            .ToListAsync();
    }



    public async Task<IEnumerable<Tag>> SearchAsync(string keyword)
    {
        var lowerKeyword = keyword.ToLower();
        return await _dbSet
            .Where(t => t.Name.ToLower().Contains(lowerKeyword) ||
                       (t.Description != null && t.Description.ToLower().Contains(lowerKeyword)) ||
                       (t.Address != null && t.Address.ToLower().Contains(lowerKeyword)))
            .OrderBy(t => t.Name)
            .ToListAsync();
    }

    #endregion

    #region Tag 特定操作

    public async Task<bool> MoveToTagTableAsync(int tagId, int? newGroupId)
    {
        var tag = await _dbSet.FindAsync(tagId);
        if (tag == null)
            return false;

        // 如果新组ID不为null，验证组是否存在
        if (newGroupId.HasValue)
        {
            var groupExists = await _context.TagGroups.AnyAsync(g => g.Id == newGroupId.Value && g.GroupType == TagGroupType.TagTable);
            if (!groupExists)
                return false;
        }

        tag.TagGroupId = newGroupId;
        return true;
    }

    public async Task<int> MoveTagsToTagTableAsync(IEnumerable<int> tagIds, int? newGroupId)
    {
        var tags = await _dbSet.Where(t => tagIds.Contains(t.Id)).ToListAsync();
        
        // 如果新组ID不为null，验证组是否存在
        if (newGroupId.HasValue)
        {
            var groupExists = await _context.TagGroups.AnyAsync(g => g.Id == newGroupId.Value && g.GroupType == TagGroupType.TagTable);
            if (!groupExists)
                return 0;
        }

        foreach (var tag in tags)
        {
            tag.TagGroupId = newGroupId;
        }

        return tags.Count;
    }

    public async Task<Tag?> CopyToTagTableAsync(int sourceTagId, int? targetGroupId)
    {
       return await Task.FromResult<Tag?>(null);
    }

    public async Task<bool> IsNameExistsAsync(string name, int? excludeId = null)
    {
        var query = _dbSet.Where(t => t.Name == name);
        if (excludeId.HasValue)
        {
            query = query.Where(t => t.Id != excludeId.Value);
        }
        return await query.AnyAsync();
    }

    public async Task<Tag?> GetWithGroupAsync(int id)
    {
        return await _dbSet
            .Include(t => t.TagGroup)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<IEnumerable<Tag>> GetWithGroupAsync(IEnumerable<int> ids)
    {
        return await _dbSet
            .Include(t => t.TagGroup)
            .Where(t => ids.Contains(t.Id))
            .OrderBy(t => t.Name)
            .ToListAsync();
    }


    #endregion
}
