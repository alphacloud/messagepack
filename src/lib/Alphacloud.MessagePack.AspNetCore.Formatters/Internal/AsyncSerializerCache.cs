namespace Alphacloud.MessagePack.AspNetCore.Formatters
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;
    using global::MessagePack;
    using JetBrains.Annotations;


    internal class AsyncSerializerCache
    {
        readonly object _lock = new object();
        volatile IReadOnlyDictionary<Type, IAsyncSerializationAdapter> _cache = new Dictionary<Type, IAsyncSerializationAdapter>(0);

        public static AsyncSerializerCache Instance => Cached.SerializerCache;

        /// <inheritdoc />
        AsyncSerializerCache()
        {
        }

        public IAsyncSerializationAdapter Get([NotNull] Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));

            if (!_cache.TryGetValue(type, out var adapter))
            {
                lock (_lock)
                {
                    if (_cache.TryGetValue(type, out adapter)) return adapter;
                    var newCache = new Dictionary<Type, IAsyncSerializationAdapter>(_cache.Count + 1);
                    foreach (var kvp in _cache)
                    {
                        newCache[kvp.Key] = kvp.Value;
                    }

                    var adapterType = typeof(TypedAdapter<>).MakeGenericType(type);
                    adapter = (IAsyncSerializationAdapter) Activator.CreateInstance(adapterType);
                    newCache[type] = adapter;
                    _cache = newCache;
                }
            }

            return adapter;
        }


        static class Cached
        {
            public static readonly AsyncSerializerCache SerializerCache = new AsyncSerializerCache();
        }
    }


    public interface IAsyncSerializationAdapter
    {
        Task SerializeAsync(Stream stream, object obj, IFormatterResolver formatterResolver);
        Task<object> DeserializeAsync(Stream stream, IFormatterResolver formatterResolver);
    }


    public class TypedAdapter<T> : IAsyncSerializationAdapter
    {
        public Task SerializeAsync(Stream stream, object obj, IFormatterResolver formatterResolver)
        {
            return MessagePackSerializer.SerializeAsync(stream, (T) obj, formatterResolver);
        }

        public async Task<object> DeserializeAsync(Stream stream, IFormatterResolver formatterResolver)
        {
            return await MessagePackSerializer.DeserializeAsync<T>(stream, formatterResolver).ConfigureAwait(false);
        }
    }
}
