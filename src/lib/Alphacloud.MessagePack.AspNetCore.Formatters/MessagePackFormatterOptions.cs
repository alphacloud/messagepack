using MessagePack;

namespace Alphacloud.MessagePack.AspNetCore.Formatters
{
    /// <summary>
    ///     MessagePack MVC formatter options.
    /// </summary>
    public class MessagePackFormatterOptions
    {
        public IFormatterResolver FormatterResolver { get; set; }
    }
}