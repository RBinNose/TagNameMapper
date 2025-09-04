using System;
using Microsoft.Extensions.Logging;

namespace TagNameMapper.Models.Logger;

public class Logger : ILogger
{
    private readonly Action<string> _logAction;
    private readonly string _categoryName;
    // 构造函数，注入日志处理委托和日志分类名
    public Logger(Action<string> logAction, string categoryName)
    {
        _logAction = logAction;
        _categoryName = categoryName;
    }

    private class NullScope : IDisposable
    {
        public void Dispose() { }
    }

    private static readonly IDisposable _nullScope = new NullScope();

    public IDisposable BeginScope<TState>(TState state) where TState : notnull
    {
        return _nullScope;
    }
    /// 控制是否启用某个日志等级，这里统一返回 true 表示全部启用
    public bool IsEnabled(LogLevel logLevel)
    {
        return true; // 可以根据 logLevel 决定是否启用
    }

    /// <summary>
    /// 日志的核心处理逻辑
    /// </summary>
    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        // 检查是否启用该等级
        if (!IsEnabled(logLevel)) return;

        // 使用 formatter 将日志内容转为字符串
        string message = formatter(state, exception);

        // 构造最终日志字符串，包含等级、分类、异常等
        string logText = $"[{DateTime.Now:HH:mm:ss}] [{logLevel}] [{_categoryName}] {message}";

        if (exception != null)
        {
            logText += $" | Exception: {exception.Message}";
        }

        // 调用日志处理委托，通常是将日志推送到 UI
        _logAction?.Invoke(logText);
    }
}
