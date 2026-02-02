using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Game.Scripts.UI_Utils
{
    public sealed class ItemsPool<T> where T : Component
    {
        private readonly T _prefab;
        private readonly Transform _parent;

        private readonly List<T> _instances = new();

        public IReadOnlyList<T> Instances => _instances;

        public ItemsPool(T prefab, Transform parent)
        {
            _prefab = prefab;
            _parent = parent;
        }

        private void ChangeCount(int count)
        {
            if (count < 0) count = 0;

            if (_instances.Count == count)
                return;

            if (_instances.Count > count)
            {
                for (int i = _instances.Count - 1; i >= count; i--)
                {
                    if (_instances[i] != null)
                        Object.Destroy(_instances[i].gameObject);

                    _instances.RemoveAt(i);
                }

                return;
            }

            for (int i = _instances.Count; i < count; i++)
            {
                T instance = _parent != null
                    ? Object.Instantiate(_prefab, _parent)
                    : Object.Instantiate(_prefab);

                _instances.Add(instance);
            }
        }

        public IEnumerable<(int Index, TItem Item, T View)> Change<TItem>(IReadOnlyList<TItem> items)
        {
            int count = items?.Count ?? 0;
            ChangeCount(count);

            for (int i = 0; i < count; i++)
            {
                var view = _instances[i];
                if (view == null)
                    continue;
                if (items == null || items[i] == null)
                    continue;

                yield return (i, items[i], view);
            }
        }

        public IEnumerable<(int Index, T View)> Change(int count)
        {
            ChangeCount(count);

            for (int i = 0; i < count; i++)
            {
                var view = _instances[i];
                if (view == null)
                    continue;

                yield return (i, view);
            }
        }

        public void Clear()
        {
            foreach (var t in _instances)
            {
                if (t != null)
                    Object.Destroy(t.gameObject);
            }

            _instances.Clear();
        }
    }
}
