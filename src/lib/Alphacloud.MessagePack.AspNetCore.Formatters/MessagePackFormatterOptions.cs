namespace Alphacloud.MessagePack.AspNetCore.Formatters
{
    using System.Collections.Generic;
    using global::MessagePack;
    using JetBrains.Annotations;
    using Microsoft.AspNetCore.Mvc;


    /// <summary>
    ///     MessagePack MVC formatter options.
    /// </summary>
    [PublicAPI]
    public class MessagePackFormatterOptions
    {
        /// <summary>
        ///     Default media type to use for content negotiation.
        /// </summary>
        public const string DefaultContentType = "application/x-msgpack";

        /// <summary>
        ///     Default file extension.
        /// </summary>
        public const string DefaultFileExtension = "msgpack";

        /// <summary>
        ///     Associated media types (default <see cref="DefaultContentType" />).
        /// </summary>
        public HashSet<string> MediaTypes { get; } = new HashSet<string> {DefaultContentType};

        /// <summary>
        ///     Associated format mappings (default <see cref="DefaultFileExtension" />).
        ///     <seealso cref="MvcOptions.FormatterMappings" />
        /// </summary>
        /// <remarks>
        ///     See
        ///     https://docs.microsoft.com/en-us/aspnet/core/web-api/advanced/formatting?view=aspnetcore-2.2#response-format-url-mappings
        ///     for further details.
        /// </remarks>
        public HashSet<string> FileExtensions { get; } = new HashSet<string> {DefaultFileExtension};

        /// <summary>
        ///     Formatter resolver.
        /// </summary>
        public IFormatterResolver FormatterResolver { get; set; }
    }
}
