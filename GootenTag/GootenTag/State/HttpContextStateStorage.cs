using System.Web;

namespace GootenTag.State
{
    /// <summary>
    /// State is added to HttpContext so that it can be rendered in the page at the end of the request.
    /// </summary>
    public class HttpContextStateStorage : IStateStorage
    {
        public T Get<T>(string key)
        {
            return (T)HttpContext.Current.Items[key];
        }

        public void Set<T>(string key, T value)
        {
            HttpContext.Current.Items[key] = value;
        }

        public void Remove(string key)
        {
            HttpContext.Current.Items.Remove(key);
        }
    }
}