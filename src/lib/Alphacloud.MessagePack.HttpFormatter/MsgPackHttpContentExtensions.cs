﻿namespace Alphacloud.MessagePack.HttpFormatter
{
    using System.Net.Http.Formatting;


    /// <summary>
    ///     Extension methods to allow strongly typed objects to be read from <see cref="T:System.Net.Http.HttpContent" />
    ///     instances using MsgPack format.
    /// </summary>
    [PublicAPI]
    public static class MsgPackHttpContentExtensions
    {
        static readonly MediaTypeFormatter[] MsgpackOnlyFormatter =
        {
            MsgPackHttpClientExtensions.DefaultFormatter
        };

        /// <summary>
        ///     Deserialize MsgPack response into specified type <typeparamref name="T" />.
        /// </summary>
        /// <remarks><see cref="MsgPackHttpClientExtensions.DefaultFormatter" /> is used to deserialize response.</remarks>
        /// <typeparam name="T">The type of object to read.</typeparam>
        /// <returns>A task object representing reading the content as an object of the specified type.</returns>
        public static Task<T> ReadAsMsgPackAsync<T>(
            this HttpContent content,
            CancellationToken cancellationToken = default)
        {
            return content.ReadAsAsync<T>(MsgpackOnlyFormatter, cancellationToken);
        }
    }
}
