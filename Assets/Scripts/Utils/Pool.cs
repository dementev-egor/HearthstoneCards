using System;
using System.Collections.Generic;

namespace Utils
{
    public class Pool<T> where T : class, IFactoryItem, new()
    {
        private List<T> _inactive = new List<T>();
        private List<T> _active = new List<T>();

        private Func<T> _constructor;

        public T this[int i] => _active[i];
        public int Count => _active.Count;

        public Pool()
        {
            _constructor = () => new T();
        }

        public Pool(Func<T> constructor)
        {
            _constructor = constructor;
        }
        
        public T Get()
        {
            T item;

            if (_inactive.Count > 0)
            {
                item = _inactive[0];
                _inactive.RemoveAt(0);
            }
            else
            {
                item = _constructor();
            }

            _active.Add(item);
            item.Activate();
            return item;
        }

        public void Deactivate(T item)
        {
            var i = _active.FindIndex(item.Equals);

            if (i < 0)
                throw new Exception($"Can't find index of item {item}");

            _active.RemoveAt(i);
            _inactive.Add(item);
            item.Deactivate();
        }

        public void DeactivateAll()
        {
            while (_active.Count > 0)
            {
                var item = _active[0];
                _active.RemoveAt(0);
                _inactive.Add(item);
                item.Deactivate();
            }
        }
    }
}