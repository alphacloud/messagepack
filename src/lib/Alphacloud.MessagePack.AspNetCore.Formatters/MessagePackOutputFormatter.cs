namespace Alphacloud.MessagePack.AspNetCore.Formatters
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using global::MessagePack;
    using JetBrains.Annotations;
    using Microsoft.AspNetCore.Mvc.Formatters;


    /// <summary>
    ///     MVC output formatter.
    /// </summary>
    public class MessagePackOutputFormatter : OutputFormatter
    {
        static readonly byte[] NilBuffer = {MessagePackCode.Nil};
        readonly IFormatterResolver _resolver;

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="resolver">Contract resolver.</param>
        /// <param name="mediaTypes">Supported media types.</param>
        public MessagePackOutputFormatter([NotNull] IFormatterResolver resolver, ICollection<string> mediaTypes)
        {
            _resolver = resolver ?? throw new ArgumentNullException(nameof(resolver));
            if (mediaTypes == null) throw new ArgumentNullException(nameof(mediaTypes));
            if (mediaTypes.Count == 0) throw new ArgumentException("Media type must be specified.", nameof(mediaTypes));

            foreach (var mediaType in mediaTypes)
            {
                SupportedMediaTypes.Add(mediaType);
            }
        }

        /// <inheritdoc />
        public override async Task WriteResponseBodyAsync(OutputFormatterWriteContext context)
        {
            if (context.ObjectType == typeof(object))
            {
                if (context.Object == null)
                {
                    await context.HttpContext.Response.Body.WriteAsync(NilBuffer, 0, 1, context.HttpContext.RequestAborted).ConfigureAwait(false);
                }

                await AsyncSerializerCache.Instance.Get(context.Object.GetType())
                    .SerializeAsync(context.HttpContext.Response.Body, context.Object, _resolver)
                    .ConfigureAwait(false);
            }

            await AsyncSerializerCache.Instance.Get(context.ObjectType).SerializeAsync(context.HttpContext.Response.Body, context.Object, _resolver)
                .ConfigureAwait(false);
        }
    }
}
