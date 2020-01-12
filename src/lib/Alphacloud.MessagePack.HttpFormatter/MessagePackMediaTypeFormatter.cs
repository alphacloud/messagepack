namespace Alphacloud.MessagePack.WebApi.Client
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
        static readonly object _lock = new object();
        static readonly IDictionary<Type, object> _valueTypeDefaults = new Dictionary<Type, object>(16);

        /// <summary>
        ///     MessagePack media type.
        /// </summary>
        [PublicAPI] public const string DefaultMediaType = "application/x-msgpack";

        [NotNull] readonly MessagePackSerializerOptions _options;
        [NotNull] ReadableTypesCache _readableTypesCache;

        /// <inheritdoc />
        public MessagePackMediaTypeFormatter([NotNull] MessagePackSerializerOptions options, [NotNull] ICollection<string> mediaTypes)
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
        public MessagePackMediaTypeFormatter([NotNull] MessagePackMediaTypeFormatter formatter)
            : base(formatter)
        {
            if (formatter == null) throw new ArgumentNullException(nameof(formatter));
            _options = formatter._options;
            _readableTypesCache = formatter._readableTypesCache;
        }

        /// <inheritdoc />
        public override bool CanReadType(Type type)
        {
            return _readableTypesCache.CanRead(type);
        }

        /// <inheritdoc />
        public override bool CanWriteType(Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            return true;
        }

        /// <inheritdoc />
        public override async Task<object> ReadFromStreamAsync(
            Type type, Stream readStream, HttpContent content, IFormatterLogger formatterLogger)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (readStream == null) throw new ArgumentNullException(nameof(readStream));
            if (content == null) throw new ArgumentNullException(nameof(content));

            long? contentLength = content?.Headers?.ContentLength;
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
            catch (Exception exception)
            {
                if (formatterLogger == null) throw;

                formatterLogger.LogError(string.Empty, exception);
                return GetDefaultForType(type);
            }
        }

        /// <inheritdoc />
        public override Task WriteToStreamAsync(
            Type type, object value, Stream writeStream, HttpContent content,
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

        static object GetDefaultForType(Type type)
        {
            if (!type.IsValueType) return null;

            // ReSharper disable once InconsistentlySynchronizedField
            if (_valueTypeDefaults.TryGetValue(type, out var def)) return def;
            lock (_lock)
            {
                if (_valueTypeDefaults.TryGetValue(type, out def)) return def;
                def = Activator.CreateInstance(type);
                _valueTypeDefaults[type] = def;
                return def;
            }
        }
    }
}
