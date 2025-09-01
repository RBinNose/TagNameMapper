using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using System.Linq;
using Avalonia.Markup.Xaml;
using TagNameMapper.Views;
using TagNameMapper.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using TagNameMapper.Models.EFCore.Extensions;
using System;
using System.Threading.Tasks;
using System.Text;

namespace TagNameMapper;

public partial class App : Application
{
    private IServiceProvider? _serviceProvider;

    public override void Initialize()
    {
        // 设置控制台编码为UTF-8，解决中文乱码
        Console.OutputEncoding = Encoding.UTF8;
        Console.InputEncoding = Encoding.UTF8;
        
        AvaloniaXamlLoader.Load(this);
        
        // 配置依赖注入
        ConfigureServices();
    }

    /// <summary>
    /// 配置依赖注入服务
    /// </summary>
    private void ConfigureServices()
    {
        var services = new ServiceCollection();
        
        // 注册数据访问层服务（使用默认连接字符串）
        services.AddDataAccessServices();
        
        // 注册ViewModels
        services.AddTransient<MainWindowViewModel>();
        
        // 构建服务提供者
        _serviceProvider = services.BuildServiceProvider();
        
        // 确保数据库已创建（异步执行）
        Task.Run(async () =>
        {
            try
            {
                await _serviceProvider.EnsureDatabaseCreatedAsync();
            }
            catch (Exception ex)
            {
                // 记录错误或处理异常
                Console.WriteLine($"数据库初始化失败: {ex.Message}");
            }
        });
    }

    public override void OnFrameworkInitializationCompleted()
    {
        // Remove Avalonia data validation.
        // Without this line you will get duplicate validations from both Avalonia and CT
        BindingPlugins.DataValidators.RemoveAt(0);

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var mainWindow = new MainWindow();
            // 使用依赖注入获取ViewModel
            mainWindow.DataContext = _serviceProvider?.GetRequiredService<MainWindowViewModel>();
            desktop.MainWindow = mainWindow;
        }

        base.OnFrameworkInitializationCompleted();
    }


}
