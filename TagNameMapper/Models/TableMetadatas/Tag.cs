using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CommunityToolkit.Mvvm.ComponentModel;
using TagNameMapper.Enums;


namespace TagNameMapper.Models.TableMetadatas;

[Table("Tag_Tags")]
public partial class Tag : ObservableObject
{
    [ObservableProperty][property:Key]
    private Guid _id = Guid.NewGuid(); // 默认生成新GUID

    [ObservableProperty][property:Required(ErrorMessage = "Tag Name is required")]
    private string _name = string.Empty;

    [ObservableProperty]
    private string? _description;

    [ObservableProperty]
    private VariableType _dataType= VariableType.String;

    /// <summary>
    /// 字符串或者数组长度
    /// </summary>
    [ObservableProperty]
    private int _variableLenth=1;  

    [ObservableProperty]
    private string? _address;

    [ObservableProperty]
    [property: NotMapped]
    private object? _value;

    [ObservableProperty]
    private VariableAccessPermission _accessPermission;

    [ObservableProperty]
    private Guid? _tagGroupId;

    [ObservableProperty]
    private int? _pollGroupId;


    // 导航属性
    [ForeignKey("TagGroupId")]
    public virtual TagGroup? TagGroup { get; set; }

    [ForeignKey("PollGroupId")]
    public virtual PollGroup? PollGroup { get; set; }

    // 计算属性
    [NotMapped]
    public string DisplayName => !string.IsNullOrEmpty(Name) ? Name : $"Tag_{Id}";

    [NotMapped]
    public string GroupPath => TagGroup?.FullPath ?? "Root";
}
