namespace Alphacloud.MessagePack.AspNetCore.Formatters
{
    using System;
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
        ///     Associated format mappings (default <see cref="DefaultFileExtension" />) (case-insensitive).
        ///     <seealso cref="MvcOptions.FormatterMappings" />
        /// </summary>
        /// <remarks>
        ///     See
        ///     https://docs.microsoft.com/en-us/aspnet/core/web-api/advanced/formatting?view=aspnetcore-2.2#response-format-url-mappings
        ///     for further details.
        /// </remarks>
        public HashSet<string> FileExtensions { get; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase) {DefaultFileExtension};

        /// <summary>
        ///     Formatter resolver.
        /// </summary>
        public IFormatterResolver FormatterResolver { get; set; }

        /// <summary>
        ///     Compression scheme to apply to serialized sequences, <see cref="MessagePackSerializerOptions.Compression" />.
        /// </summary>
        public MessagePackCompression Compression { get; set; }

        /// <summary>
        ///     Serialize using old specification.
        ///     See <see cref="MessagePackWriter.OldSpec" /> and <see cref="MessagePackSerializerOptions.OldSpec" /> for details.
        /// </summary>
        /// <remarks>
        ///     Reading always supports both new and old specifications.
        /// </remarks>
        public bool? UseOldSpecification { get; set; }

        /// <summary>
        ///     Value indicating whether serialization should omit assembly version, culture and public key token metadata when
        ///     using the typeless formatter,
        ///     default is <c>false</c>.
        ///     See <see cref="MessagePackSerializerOptions.OmitAssemblyVersion" /> for details.
        /// </summary>
        public bool OmitAssemblyVersion { get; set; }

        /// <summary>
        ///     Allows deserializer to instantiate types from an assembly with a different version if a matching version cannot be
        ///     found, default is <c>false</c>.
        ///     See <see cref="MessagePackSerializerOptions.AllowAssemblyVersionMismatch" />
        /// </summary>
        public bool AllowAssemblyVersionMismatch { get; set; }
    }
}
