using System;
using Avalonia.Threading;
using Microsoft.Extensions.Logging;
using TagNameMapper.Models.Common;

namespace TagNameMapper.Models.Logger;

public class LoggerProvider : ILoggerProvider
{
    //资源清理标记位防止反复清理
    private bool _disposed = false;

    //日志信息存储位置
    private LimitedObservableCollection<string> _logMessages;
    public LimitedObservableCollection<string> LogMessages
    {
        get { return _logMessages; }
    }
    //构造函数
    public LoggerProvider()
    {
        _logMessages=new LimitedObservableCollection<string>(1000,100);
    }

    /// <summary>
    /// 创建对应分类的 Logger 实例
    /// </summary>
    /// <param name="categoryName">日志分类（如类名）</param>
    public ILogger CreateLogger(string categoryName)
    {
        return new Logger((message) =>
        {
            if ( message != null && LogMessages != null)
            {
                // 如果不在 UI 线程中，使用 Invoke 将更新操作调度到 UI 线程
                Dispatcher.UIThread.Post(() => LogMessages.Add(message));
            }
        }, categoryName);
    }

    /// <summary>
    /// 实现接口要求的 Dispose（这里没用资源所以为空）
    /// </summary>
    public void Dispose()
    {
        if (!_disposed)
        {
      
            // 清理其他资源（如果有）
            // 比如 Logger 资源清理，如果 Logger 实现了 IDisposable，调用其 Dispose
            // _logger?.Dispose();

            // 标记为已清理
            _disposed = true;
        }
    }
}