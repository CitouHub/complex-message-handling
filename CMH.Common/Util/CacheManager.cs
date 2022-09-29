namespace CMH.Common.Util
{
    public interface ICacheManager
    {
        void Set(string key, object? value);
        T? Get<T>(string key);
        void Clear();
    }
    public class CacheManager : ICacheManager
    {
        public void Set(string key, object? value)
        {
            if(value != null)
            {
                Cache.Items[key] = value;
            }
        }

        public T? Get<T>(string key)
        {
            if (Cache.Items.ContainsKey(key))
            {
                return (T)Cache.Items[key];
            }

            return default;
        }

        public void Clear()
        {
            Cache.Items = new();
        }
    }
}
