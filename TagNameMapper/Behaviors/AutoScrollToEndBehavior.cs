using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Xaml.Interactivity;
using System.Collections;
using System.Collections.Specialized;


namespace TagNameMapper.Behaviors;

public sealed class AutoScrollToEndBehavior : Behavior<ListBox>
{
    private INotifyCollectionChanged? _source;

    protected override void OnAttached()
    {
        base.OnAttached();

        var lb = AssociatedObject!;

        // 初次挂载：尝试订阅当前 Items（已绑定的话会有 Items）
        TryHook(lb.Items);

        // 监听 ItemsSource 属性变化
        lb.PropertyChanged += OnListBoxPropertyChanged;
    }

    protected override void OnDetaching()
    {
        base.OnDetaching();
        if (AssociatedObject != null)
        {
            AssociatedObject.PropertyChanged -= OnListBoxPropertyChanged;
        }
        Unhook();
    }

    private void OnListBoxPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Property == ItemsControl.ItemsSourceProperty)
        {
            TryHook(AssociatedObject?.Items);
        }
    }

    private void TryHook(IEnumerable? items)
    {
        // 先解绑旧集合
        Unhook();

        // 只对实现了 INotifyCollectionChanged 的集合有效
        _source = items as INotifyCollectionChanged;
        if (_source is not null)
        {
            _source.CollectionChanged += OnCollectionChanged;
        }
    }

    private void Unhook()
    {
        if (_source is not null)
        {
            _source.CollectionChanged -= OnCollectionChanged;
            _source = null;
        }
    }

    private void OnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action != NotifyCollectionChangedAction.Add || e.NewItems is null || e.NewItems.Count == 0)
            return;

        // 这次新增的最后一个元素
        var last = e.NewItems[e.NewItems.Count - 1];
        if (last is null) return;

        // 切回 UI 线程再滚动
        Avalonia.Threading.Dispatcher.UIThread.Post(() =>
        {
            AssociatedObject?.ScrollIntoView(last);
        });
    }

}
