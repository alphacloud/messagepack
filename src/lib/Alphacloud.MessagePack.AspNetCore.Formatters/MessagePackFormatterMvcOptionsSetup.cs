namespace Alphacloud.MessagePack.AspNetCore.Formatters
{
    using System;
    using System.Linq;
    using JetBrains.Annotations;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;


    /// <summary>
    ///     Formatting options setup.
    /// </summary>
    [UsedImplicitly]
    internal class MessagePackFormatterMvcOptionsSetup : IConfigureOptions<MvcOptions>
    {
        readonly IOptions<MessagePackFormatterOptions> _messagePackFormatterOptions;

        /// <inheritdoc />
        public MessagePackFormatterMvcOptionsSetup([NotNull] IOptions<MessagePackFormatterOptions> messagePackFormatterOptions)
        {
            _messagePackFormatterOptions = messagePackFormatterOptions ??
                throw new ArgumentNullException(nameof(messagePackFormatterOptions));
        }

        /// <inheritdoc />
        public void Configure(MvcOptions options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));

            var formatterOptions = _messagePackFormatterOptions.Value;
            var supportedMediaTypes = formatterOptions.MediaTypes
                .Where(t => !string.IsNullOrWhiteSpace(t))
                .ToArray();
            if (supportedMediaTypes.Length == 0) throw new InvalidOperationException("No supported media types were specified.");

            options.InputFormatters.Add(new MessagePackInputFormatter(formatterOptions.FormatterResolver, supportedMediaTypes));
            options.OutputFormatters.Add(
                new MessagePackOutputFormatter(formatterOptions.FormatterResolver, supportedMediaTypes));

            foreach (var fileExtension in formatterOptions.FileExtensions)
            {
                options.FormatterMappings.SetMediaTypeMappingForFormat(fileExtension, supportedMediaTypes[0]);
            }
        }
    }
}
