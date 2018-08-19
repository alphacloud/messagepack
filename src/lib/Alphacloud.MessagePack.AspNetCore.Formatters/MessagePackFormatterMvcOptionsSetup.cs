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
        readonly MessagePackFormatterOptions _messagePackFormatterOptions;

        /// <inheritdoc />
        public MessagePackFormatterMvcOptionsSetup([NotNull] MessagePackFormatterOptions messagePackFormatterOptions)
        {
            _messagePackFormatterOptions = messagePackFormatterOptions ??
                throw new ArgumentNullException(nameof(messagePackFormatterOptions));
        }

        /// <inheritdoc />
        public void Configure([NotNull] MvcOptions options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));

            var supportedMediaTypes = _messagePackFormatterOptions.MediaTypes
                .Where(t => !string.IsNullOrWhiteSpace(t))
                .Distinct().ToArray();
            if (supportedMediaTypes.Length == 0) throw new InvalidOperationException("No supported media types were specified.");

            options.InputFormatters.Add(new MessagePackInputFormatter(_messagePackFormatterOptions.FormatterResolver, supportedMediaTypes));
            options.OutputFormatters.Add(
                new MessagePackOutputFormatter(_messagePackFormatterOptions.FormatterResolver, supportedMediaTypes));
        }
    }
}
