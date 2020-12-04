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
        readonly MessagePackSerializerOptions _options;

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="options">Contract resolver.</param>
        /// <param name="mediaTypes">Supported media types.</param>
        public MessagePackOutputFormatter([NotNull] MessagePackSerializerOptions options, ICollection<string> mediaTypes)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
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
#if NETCOREAPP2_1 || NETCOREAPP2_2
            if (context.ObjectType == typeof(object))
            {
                if (context.Object == null)
                {
                    context.HttpContext.Response.Body.WriteByte(MessagePackCode.Nil);
                    return Task.CompletedTask;
                }

                return MessagePackSerializer.SerializeAsync(context.Object.GetType(), context.HttpContext.Response.Body, context.Object,
                    _options, context.HttpContext.RequestAborted);
            }

            return MessagePackSerializer.SerializeAsync(context.ObjectType, context.HttpContext.Response.Body, context.Object, _options,
                context.HttpContext.RequestAborted);
#endif

#if NETCOREAPP3_0 || NETCOREAPP3_1 || NET5_0
            var writer = context.HttpContext.Response.BodyWriter;
            if (context.ObjectType == typeof(object))
            {
                if (context.Object == null)
                {
                    var span = writer.GetSpan(1);
                    span[0] = MessagePackCode.Nil;
                    writer.Advance(1);
                }
                else
                {
                    MessagePackSerializer.Serialize(context.Object.GetType(), writer, context.Object, _options, context.HttpContext.RequestAborted);
                }
            }
            else
            {
                MessagePackSerializer.Serialize(context.ObjectType, writer, context.Object, _options, context.HttpContext.RequestAborted);
            }

            return writer.FlushAsync().AsTask();
#endif
        }
    }
}
