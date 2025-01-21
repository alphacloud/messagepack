// ReSharper disable once CheckNamespace

namespace Alphacloud.MessagePack.HttpFormatter.Internal;

class SerializableTypesCache(IFormatterResolver formatterResolver)
{
    readonly IFormatterResolver _formatterResolver = formatterResolver ?? throw new ArgumentNullException(nameof(formatterResolver));
    readonly object _lock = new();
    Dictionary<Type, bool> _serializableTypes = [];

    /// <summary>
    ///     Check whether type is readable.
    /// </summary>
    /// <param name="type">Type to check.</param>
    /// <returns></returns>
    /// <exception cref="T:System.ArgumentNullException"><paramref name="type" /> is <see langword="null" /></exception>
    public bool CanSerialize(Type type)
    {
        if (type == null)
            throw new ArgumentNullException(nameof(type));

        if (!_serializableTypes.TryGetValue(type, out bool canSerialize))
        {
            lock (_lock)
            {
                if (_serializableTypes.TryGetValue(type, out canSerialize))
                    return canSerialize;

                canSerialize = _formatterResolver.GetFormatterDynamic(type) != null;
                var readableTypes = new Dictionary<Type, bool>(_serializableTypes.Count + 1) { [type] = canSerialize };
                foreach (var readableType in _serializableTypes)
                {
                    readableTypes[readableType.Key] = readableType.Value;
                }

                _serializableTypes = readableTypes;
            }

            return canSerialize;
        }

        return canSerialize;
    }
}
