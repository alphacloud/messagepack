namespace Alphacloud.MessagePack.HttpFormatter
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Formatting;
    using System.Net.Http.Headers;
    using System.Threading;
    using System.Threading.Tasks;
    using global::MessagePack;
    using Internal;
    using JetBrains.Annotations;


    /// <summary>
    ///     MsgPack data formatter for Web Api client.
    /// </summary>
    [PublicAPI]
    public class MessagePackMediaTypeFormatter : MediaTypeFormatter
    {
        static readonly byte[] NilBuffer = {MessagePackCode.Nil};
        static readonly object Lock = new object();
        static readonly IDictionary<Type, object?> ValueTypeDefaults = new Dictionary<Type, object?>(16);

        /// <summary>
        ///     MessagePack media type.
        /// </summary>
        [PublicAPI] public const string DefaultMediaType = "application/x-msgpack";

        readonly MessagePackSerializerOptions _options;
        ReadableTypesCache _readableTypesCache;

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="options">Formatter options.</param>
        /// <param name="mediaTypes">Supported media types.</param>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="options" /> or <paramref name="mediaTypes" /> is <c>null</c>.
        /// </exception>
        public MessagePackMediaTypeFormatter(MessagePackSerializerOptions options, ICollection<string> mediaTypes)
        {
            if (mediaTypes == null) throw new ArgumentNullException(nameof(mediaTypes));
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _readableTypesCache = new ReadableTypesCache(options.Resolver);

            foreach (var mediaType in mediaTypes)
            {
                SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse(mediaType));
            }
        }

        /// <inheritdoc />
        /// <exception cref="T:System.ArgumentNullException"><paramref name="formatter"/> is <c>null</c></exception>
        public MessagePackMediaTypeFormatter(MessagePackMediaTypeFormatter formatter)
            : base(formatter)
        {
            if (formatter == null) throw new ArgumentNullException(nameof(formatter));
            _options = formatter._options;
            _readableTypesCache = formatter._readableTypesCache;
        }

        /// <inheritdoc />
        /// <exception cref="T:System.ArgumentNullException"><paramref name="type"/> is <c>null</c>.</exception>
        public override bool CanReadType(Type type)
        {
            return _readableTypesCache.CanRead(type);
        }

        /// <inheritdoc />
        /// <exception cref="T:System.ArgumentNullException"><paramref name="type"/> is <c>null</c>.</exception>
        public override bool CanWriteType(Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            return true;
        }

        /// <inheritdoc />
        /// <exception cref="T:System.ArgumentNullException"><paramref name="type"/> is <see langword="null"/></exception>
        public override async Task<object?> ReadFromStreamAsync(
            Type type, Stream readStream, HttpContent content, IFormatterLogger formatterLogger)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (readStream == null) throw new ArgumentNullException(nameof(readStream));
            if (content == null) throw new ArgumentNullException(nameof(content));

            long? contentLength = content.Headers.ContentLength;
            if (contentLength.HasValue && contentLength.GetValueOrDefault() == 0L)
            {
                return GetDefaultForType(type);
            }

            try
            {
                var result = await MessagePackSerializer.DeserializeAsync(type, readStream, _options)
                    .ConfigureAwait(false);
                return result;
            }
            // ReSharper disable once CatchAllClause
            catch (Exception exception)
            {
                formatterLogger.LogError(string.Empty, exception);
                return GetDefaultForType(type);
            }
        }

        /// <inheritdoc />
        /// <exception cref="T:System.ArgumentNullException"><paramref name="type"/> or <paramref name="writeStream"/>is <c>null</c>.</exception>
        public override Task WriteToStreamAsync(
            Type type, object? value, Stream writeStream, HttpContent content,
            TransportContext transportContext, CancellationToken cancellationToken)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (writeStream == null) throw new ArgumentNullException(nameof(writeStream));

            if (cancellationToken.IsCancellationRequested) return Task.FromCanceled(cancellationToken);

            if (value == null && type == typeof(object))
            {
                return writeStream.WriteAsync(NilBuffer, 0, 1, cancellationToken);
            }

            return MessagePackSerializer.SerializeAsync(type, writeStream, value, _options, cancellationToken);
        }

        static object? GetDefaultForType(Type type)
        {
            if (!type.IsValueType) return null;

            // ReSharper disable once InconsistentlySynchronizedField
            if (ValueTypeDefaults.TryGetValue(type, out var def)) return def;
            lock (Lock)
            {
                if (ValueTypeDefaults.TryGetValue(type, out def)) return def;
                def = Activator.CreateInstance(type);
                ValueTypeDefaults[type] = def;
                return def;
            }
        }
    }
}
