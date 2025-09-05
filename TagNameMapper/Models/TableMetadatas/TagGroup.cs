using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using TagNameMapper.Enums;

namespace TagNameMapper.Models.TableMetadatas;

[Table("Tag_Groups")]
public partial class TagGroup : ObservableObject
{
    [ObservableProperty]
    [property: Key]
    private Guid _id = Guid.NewGuid(); // 默认生成新GUID

    [ObservableProperty]
    [property: Required(ErrorMessage = "Group Name is required")]
    private string _name = string.Empty;

    // 编辑状态属性 - 简化版本
    [ObservableProperty]
    [property: NotMapped]
    private bool _isEditing;

    [ObservableProperty]
    private Guid? _parentGroupId;

    [ObservableProperty]
    private TagGroupType _groupType;

    // 导航属性
    [ForeignKey("ParentGroupId")]
    public virtual TagGroup? ParentGroup { get; set; }

    public virtual ObservableCollection<TagGroup> ChildGroups { get; set; } = new();

    public virtual ICollection<Tag> Tags { get; set; } = new List<Tag>();

    // 计算属性
    [NotMapped]
    public bool HasChildren => ChildGroups?.Any() == true;

    /// <summary>
    /// 获取完整路径（从根节点到当前节点）
    /// </summary>
    [NotMapped]
    public string FullPath
    {
        get
        {
            var pathParts = new List<string>();
            var current = this;
            
            // 向上遍历到根节点
            while (current != null)
            {
                pathParts.Insert(0, current.Name);
                current = current.ParentGroup;
            }
            
            return string.Join("/", pathParts);
        }
    }

}
