using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using TagNameMapper.Models.EFCore.Repositories;
using TagNameMapper.Models.TableMetadatas;
using TagNameMapper.Enums;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;

namespace TagNameMapper.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<MainWindowViewModel> _logger;

    [ObservableProperty]
    private ObservableCollection<TagGroup> _tagGroups = new();

    [ObservableProperty]
    private TagGroup? _selectedTagGroup;

    [ObservableProperty]
    private ObservableCollection<string> _logMessages = new();

    public MainWindowViewModel(IUnitOfWork unitOfWork, ILogger<MainWindowViewModel> logger)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        // 验证依赖注入是否成功
        _logger.LogInformation("✅ IUnitOfWork 注入成功");
        _logger.LogInformation("✅ TagRepository类型: {RepositoryType}", _unitOfWork.Tags.GetType().Name);
        _logger.LogInformation("✅ TagGroupRepository类型: {RepositoryType}", _unitOfWork.TagGroups.GetType().Name);
    }

    public async Task LoadTagGroupsAsync()
    {
        try
        {
            // 获取所有根节点（ParentGroupId 为 null 的节点）
            var rootGroups = await _unitOfWork.TagGroups.GetRootGroupsAsync();

            TagGroups.Clear();
            foreach (var group in rootGroups)
            {
                TagGroups.Add(group);
            }
            _logger.LogInformation(,"成功加载 {Count} 个根节点组", rootGroups.Count);
        }
        catch (Exception ex)
        {
            // 处理异常
            _logger.LogError(ex, "加载 TagGroups 失败: {Message}", ex.Message);
        }
    }

    // 异步命令
    [RelayCommand]
    private async Task LoadDataAsync()
    {
        await LoadTagGroupsAsync();

    }

    // 添加组
    [RelayCommand]
    private void AddGroup()
    {
            if (SelectedTagGroup == null)
            {
                _logger.LogWarning("未选中任何组，无法添加新组");
                return;
            }

            // 创建新的组
            var newGroup = new TagGroup
            {
                Name = "新组",
                ParentGroupId = SelectedTagGroup.Id,
                GroupType = TagGroupType.Folder// 设置为普通组类型
            };

            // 输出日志，确认创建成功
            _logger.LogInformation("成功创建组: {Name}, ID: {Id}, 父Name: {ParentName}", 
                newGroup.Name, newGroup.Id, SelectedTagGroup.Name);
            
    }
    // 添加变量表
    [RelayCommand]
    private void AddVariableTable()
    {
            // 检查是否有选中的项
            if (SelectedTagGroup == null)
            {
                _logger.LogWarning("未选中任何组，无法添加变量表");
                return;
            }

            // 创建新的变量表组
            var newVariableTable = new TagGroup
            {
                Name = "新变量表",
                ParentGroupId = SelectedTagGroup.Id,
                GroupType = TagGroupType.TagTable // 设置为变量表类型
            };

            // 输出日志，确认创建成功
            _logger.LogInformation("成功创建变量表: {Name}, ID: {Id}, 父Name: {ParentName}", 
                newVariableTable.Name, newVariableTable.Id, SelectedTagGroup.Name);
            
            // 这里可以添加保存到数据库的代码
            // await _unitOfWork.TagGroups.AddAsync(newVariableTable);
            // await _unitOfWork.SaveChangesAsync();
            
            // 这里可以添加更新UI树形结构的代码

    }

  
}
