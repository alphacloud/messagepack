namespace Alphacloud.MessagePack.AspNetCore.Formatters
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using global::MessagePack;
    using Internal;
    using JetBrains.Annotations;
    using Microsoft.AspNetCore.Mvc.Formatters;


    /// <summary>
    ///     MVC Input formatter.
    /// </summary>
    [PublicAPI]
    public class MessagePackInputFormatter : InputFormatter
    {
        readonly ReadableTypesCache _readableTypesCache;
        readonly MessagePackSerializerOptions _options;

        /// <inheritdoc />
        public MessagePackInputFormatter([NotNull] MessagePackSerializerOptions options, [NotNull] ICollection<string> mediaTypes)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            if (mediaTypes == null) throw new ArgumentNullException(nameof(mediaTypes));
            if (mediaTypes.Count == 0) throw new ArgumentException("Media type must be specified.", nameof(mediaTypes));
            _readableTypesCache = new ReadableTypesCache(options.Resolver);

            foreach (var mediaType in mediaTypes)
            {
                SupportedMediaTypes.Add(mediaType);
            }
        }

        /// <inheritdoc />
        public override async Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context)
        {
            var httpContext = context.HttpContext;
            var result = await MessagePackSerializer.DeserializeAsync(context.ModelType, httpContext.Request.Body, _options,
                httpContext.RequestAborted).ConfigureAwait(false);
            return InputFormatterResult.Success(result);
        }

        /// <inheritdoc />
        protected override bool CanReadType(Type type)
        {
            return _readableTypesCache.CanRead(type);
        }

    }
}
