using System;
using System.Collections.ObjectModel;

namespace TagNameMapper.Models.Common;


    public class LimitedObservableCollection<T> : ObservableCollection<T>
    {
        private readonly int _maxSize;
        private readonly int _itemsToRemove;

        // 构造函数，接受最大容量参数和要删除的项目数量
        public LimitedObservableCollection(int maxSize, int itemsToRemove = 10)
        {
            _maxSize = maxSize;
            _itemsToRemove = itemsToRemove;
        }

        // 重写 Add 方法
        public new void Add(T item)
        {
            if (Count >= _maxSize)
            {
                // 删除最早的 N 条元素
                for (int i = 0; i < _itemsToRemove && Count > 0; i++)
                {
                    RemoveAt(0);  // 删除第一个元素
                }
            }

        // 调用父类的 Add 方法，添加新项
            base.Add(item);
        }
    }


