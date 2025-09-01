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



    public MainWindowViewModel(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

        // 验证依赖注入是否成功
        Console.WriteLine("✅ IUnitOfWork 注入成功");
        Console.WriteLine($"✅ TagRepository类型: {_unitOfWork.Tags.GetType().Name}");
        Console.WriteLine($"✅ TagGroupRepository类型: {_unitOfWork.TagGroups.GetType().Name}");


    }

    //测试属性
    [ObservableProperty]
    private string _message = "Hello World!";

    // 异步命令
    [RelayCommand]
    private async Task LoadDataAsync()
    {
        try
        {
            // 测试数据库连接和UnitOfWork
            var tagGroups = await _unitOfWork.TagGroups.GetAllAsync();
            Console.WriteLine($"✅ 数据库连接成功，找到 {tagGroups.Count()} 个TagGroup");
            
            var tags = await _unitOfWork.Tags.GetAllAsync();
            Console.WriteLine($"✅ 找到 {tags.Count()} 个Tag");
            
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ 数据加载失败: {ex.Message}");
        }
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
                TagGroupId = testGroup.Id
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
