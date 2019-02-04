// ReSharper disable once CheckNamespace
namespace Alphacloud.MessagePack.Internal
{
    using System;
    using System.Collections.Immutable;
    using global::MessagePack;
    using JetBrains.Annotations;

    internal class ReadableTypesCache
    {
        readonly IFormatterResolver _formatterResolver;
        ImmutableDictionary<Type, bool> _readableTypes = ImmutableDictionary<Type, bool>.Empty;

        /// <inheritdoc />
        public ReadableTypesCache([NotNull] IFormatterResolver formatterResolver)
        {
            _formatterResolver = formatterResolver ?? throw new ArgumentNullException(nameof(formatterResolver));
        }

        public bool CanRead([NotNull] Type type)
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