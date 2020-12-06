// ReSharper disable once CheckNamespace
namespace Alphacloud.MessagePack.Internal
{
    using System;
    using System.Collections.Immutable;
    using global::MessagePack;

    internal class ReadableTypesCache
    {
        readonly IFormatterResolver _formatterResolver;
        ImmutableDictionary<Type, bool> _readableTypes = ImmutableDictionary<Type, bool>.Empty;

        public ReadableTypesCache(IFormatterResolver formatterResolver)
        {
            _formatterResolver = formatterResolver ?? throw new ArgumentNullException(nameof(formatterResolver));
        }

        /// <summary>
        /// Check whether type is readable.
        /// </summary>
        /// <param name="type">Type to check.</param>
        /// <returns></returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="type"/> is <see langword="null"/></exception>
        public bool CanRead(Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (!_readableTypes.TryGetValue(type, out bool canRead))
            {
                canRead = _formatterResolver.GetFormatterDynamic(type) != null;
                _readableTypes = _readableTypes.Add(type, canRead);

                return canRead;
            }

            return canRead;
        }
    }
}