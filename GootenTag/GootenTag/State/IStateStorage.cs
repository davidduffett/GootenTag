namespace GootenTag.State
{
    /// <summary>
    /// By default, state is stored using <see cref="HttpContextStateStorage"/>.
    /// Switching to <see cref="InMemoryStateStorage"/> is useful for unit testing.
    /// </summary>
    public interface IStateStorage
    {
        T Get<T>(string key);
        void Set<T>(string key, T value);
        void Remove(string key);
    }
}