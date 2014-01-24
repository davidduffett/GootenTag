using System.Collections;

namespace GootenTag.State
{
    /// <summary>
    /// Alternative storage implementation useful for unit testing.
    /// </summary>
    public class InMemoryStateStorage : IStateStorage
    {
        private readonly IDictionary _items = new Hashtable();

        public T Get<T>(string key)
        {
            return (T)_items[key];
        }

        public void Set<T>(string key, T value)
        {
            _items[key] = value;
        }

        public void Remove(string key)
        {
            _items.Remove(key);
        }
    }
}