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
        /// <summary>
        ///     MessagePack media type.
        /// </summary>
        [PublicAPI] public const string DefaultMediaType = "application/x-msgpack";

        static readonly Task<object> NullResult = Task.FromResult<object>(null);
        [NotNull] readonly IFormatterResolver _resolver;
        ReadableTypesCache _readableTypesCache;

        /// <inheritdoc />
        public MessagePackMediaTypeFormatter([NotNull] IFormatterResolver resolver, [NotNull] ICollection<string> mediaTypes)
        {
            if (mediaTypes == null) throw new ArgumentNullException(nameof(mediaTypes));
            _resolver = resolver ?? throw new ArgumentNullException(nameof(resolver));
            _readableTypesCache = new ReadableTypesCache(resolver);

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
            _resolver = formatter._resolver;
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
        public override Task<object> ReadFromStreamAsync(
            Type type, Stream readStream, HttpContent content, IFormatterLogger formatterLogger)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (readStream == null) throw new ArgumentNullException(nameof(readStream));
            if (content == null) throw new ArgumentNullException(nameof(content));

            long? contentLength = content?.Headers?.ContentLength;
            if (contentLength.HasValue && contentLength.GetValueOrDefault() == 0L)
            {
                return GetDefaultAsyncResultForType(type);
            }

            try
            {
                var result = MessagePackSerializer.NonGeneric.Deserialize(type, readStream, _resolver);
                return result != null
                    ? Task.FromResult(result)
                    : NullResult;
            }
            catch (Exception exception)
            {
                if (formatterLogger == null) throw;

                formatterLogger.LogError(string.Empty, exception);
                return GetDefaultAsyncResultForType(type);
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
                writeStream.WriteByte(MessagePackCode.Nil);
            }
            else
            {
                MessagePackSerializer.NonGeneric.Serialize(type, writeStream, value, _resolver);
            }

            return Task.CompletedTask;
        }

        static Task<object> GetDefaultAsyncResultForType(Type type)
        {
            return type.IsValueType
                ? Task.FromResult(Activator.CreateInstance(type))
                : NullResult;
        }
    }
}
