namespace Alphacloud.MessagePack.AspNetCore.Formatters
{
    using System;
    using System.Collections.Generic;
    using System.IO;
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
        readonly IFormatterResolver _resolver;

        /// <inheritdoc />
        public MessagePackInputFormatter([NotNull] IFormatterResolver resolver, [NotNull] ICollection<string> mediaTypes)
        {
            _resolver = resolver ?? throw new ArgumentNullException(nameof(resolver));
            if (mediaTypes == null) throw new ArgumentNullException(nameof(mediaTypes));
            if (mediaTypes.Count == 0) throw new ArgumentException("Media type must be specified.", nameof(mediaTypes));
            _readableTypesCache = new ReadableTypesCache(resolver);

            foreach (var mediaType in mediaTypes)
            {
                SupportedMediaTypes.Add(mediaType);
            }
        }

        /// <inheritdoc />
        public override async Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context)
        {
            var request = context.HttpContext.Request;

            var result = await AsyncSerializerCache.Instance.Get(context.ModelType).DeserializeAsync(request.Body, _resolver);
            return InputFormatterResult.Success(result);
        }

        /// <inheritdoc />
        protected override bool CanReadType(Type type)
        {
            return _readableTypesCache.CanRead(type);
        }

    }
}
