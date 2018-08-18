namespace Alphacloud.MessagePack.AspNetCore.Formatters
{
    using System.Collections.Generic;
    using global::MessagePack;
    using JetBrains.Annotations;


    /// <summary>
    ///     MessagePack MVC formatter options.
    /// </summary>
    [PublicAPI]
    public class MessagePackFormatterOptions
    {
        /// <summary>
        ///     Supported media type.
        /// </summary>
        public const string DefaultContentType = "application/x-msgpack";

        /// <summary>
        ///     Associated media types (default <see cref="DefaultContentType" />).
        /// </summary>
        public List<string> MediaTypes { get; } = new List<string>(new[] {DefaultContentType});

        /// <summary>
        ///     Formatter resolver.
        /// </summary>
        public IFormatterResolver FormatterResolver { get; set; }
    }
}
