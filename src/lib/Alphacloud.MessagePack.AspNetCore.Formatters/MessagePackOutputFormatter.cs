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
        public override Task WriteResponseBodyAsync(OutputFormatterWriteContext context)
        {
            if (context.ObjectType == typeof(object))
            {
                if (context.Object == null)
                {
                    context.HttpContext.Response.Body.WriteByte(MessagePackCode.Nil);
                }
                else
                {
                    // infer type from the instance
                    MessagePackSerializer.NonGeneric.Serialize(context.Object.GetType(),
                        context.HttpContext.Response.Body, context.Object, _resolver);
                }
            }
            else
                MessagePackSerializer.NonGeneric.Serialize(context.ObjectType, context.HttpContext.Response.Body, context.Object, _resolver);

            return Task.CompletedTask;
        }
    }
}
