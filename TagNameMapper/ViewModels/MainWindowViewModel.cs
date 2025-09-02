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

namespace TagNameMapper.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    private readonly IUnitOfWork _unitOfWork;

    [ObservableProperty]
    private ObservableCollection<TagGroup> _tagGroups = new();



    public MainWindowViewModel(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

        // 验证依赖注入是否成功
        Console.WriteLine("✅ IUnitOfWork 注入成功");
        Console.WriteLine($"✅ TagRepository类型: {_unitOfWork.Tags.GetType().Name}");
        Console.WriteLine($"✅ TagGroupRepository类型: {_unitOfWork.TagGroups.GetType().Name}");


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
        }
        catch (Exception ex)
        {
            // 处理异常
            Console.WriteLine($"加载 TagGroups 失败: {ex.Message}");
        }
    }

    // 异步命令
    [RelayCommand]
    private async Task LoadDataAsync()
    {
        await LoadTagGroupsAsync();

    }

    /// <summary>
    /// 测试创建数据的方法
    /// </summary>
    public async Task TestCreateDataAsync()
    {
        try
        {
            // 创建测试TagGroup
            var testGroup = new TagGroup
            {
                Name = "测试组_" + DateTime.Now.ToString("HHmmss"),
                GroupType = TagGroupType.Folder,
                ParentGroupId = null
            };
            
            await _unitOfWork.TagGroups.AddAsync(testGroup);
            await _unitOfWork.SaveChangesAsync();
            
            Console.WriteLine($"✅ 成功创建TagGroup: {testGroup.Name}");
            
            // 创建测试Tag
            var testTag = new Tag
            {
                Name = "测试标签_" + DateTime.Now.ToString("HHmmss"),
                Address = "PLC.Test" + DateTime.Now.ToString("HHmmss"),
            };
            
            await _unitOfWork.Tags.AddAsync(testTag);
            await _unitOfWork.SaveChangesAsync();
            
            Console.WriteLine($"✅ 成功创建Tag: {testTag.Name}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ 创建数据失败: {ex.Message}");
        }
    }
}
