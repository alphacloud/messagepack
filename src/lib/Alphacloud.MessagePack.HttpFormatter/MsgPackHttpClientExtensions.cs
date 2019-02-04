namespace Alphacloud.MessagePack.WebApi.Client
{
    using System;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using global::MessagePack.Resolvers;
    using JetBrains.Annotations;


    /// <summary>
    ///     MessagePack formatting support for <see cref="HttpClient" />
    /// </summary>
    public static class MsgPackHttpClientExtensions
    {
        /// <summary>
        ///     Default formatter (uses <see cref="ContractlessStandardResolver" />).
        /// </summary>
        [PublicAPI]
        public static readonly MessagePackMediaTypeFormatter DefaultFormatter = new MessagePackMediaTypeFormatter(ContractlessStandardResolver.Instance,
            new[] {MessagePackMediaTypeFormatter.DefaultMediaType});

        /// <summary>
        ///     Sends a POST request as an asynchronous operation to the specified Uri with the given <paramref name="value" />
        ///     serialized using MessagePack format.
        /// </summary>
        /// <remarks>
        ///     This method uses a default instance of <see cref="MessagePackMediaTypeFormatter" /> (
        ///     <see cref="ContractlessStandardResolver" />).
        /// </remarks>
        /// <typeparam name="T">The type of <paramref name="value" />.</typeparam>
        /// <param name="client">The client used to make the request.</param>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <param name="value">The value that will be placed in the request's entity body.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A task object representing the asynchronous operation.</returns>
        public static Task<HttpResponseMessage> PostAsMsgPackAsync<T>(
            this HttpClient client, Uri requestUri, T value, CancellationToken cancellationToken)
        {
            return client.PostAsync(requestUri, value, DefaultFormatter, cancellationToken);
        }

        /// <summary>
        ///     Sends a POST request as an asynchronous operation to the specified Uri with the given <paramref name="value" />
        ///     serialized using MessagePack format.
        /// </summary>
        /// <remarks>
        ///     This method uses a default instance of <see cref="MessagePackMediaTypeFormatter" /> (
        ///     <see cref="ContractlessStandardResolver" />).
        /// </remarks>
        /// <typeparam name="T">The type of <paramref name="value" />.</typeparam>
        /// <param name="client">The client used to make the request.</param>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <param name="value">The value that will be placed in the request's entity body.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A task object representing the asynchronous operation.</returns>
        public static Task<HttpResponseMessage> PostAsMsgPackAsync<T>(
            this HttpClient client, string requestUri, T value, CancellationToken cancellationToken)
        {
            return client.PostAsync(requestUri, value, DefaultFormatter, cancellationToken);
        }

        /// <summary>
        ///     Sends a PUT request as an asynchronous operation to the specified Uri with the given <paramref name="value" />
        ///     serialized using MessagePack format.
        /// </summary>
        /// <remarks>
        ///     This method uses a default instance of <see cref="MessagePackMediaTypeFormatter" /> (
        ///     <see cref="ContractlessStandardResolver" />).
        /// </remarks>
        /// <typeparam name="T">The type of <paramref name="value" />.</typeparam>
        /// <param name="client">The client used to make the request.</param>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <param name="value">The value that will be placed in the request's entity body.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A task object representing the asynchronous operation.</returns>
        public static Task<HttpResponseMessage> PutAsMsgPackAsync<T>(
            this HttpClient client, Uri requestUri, T value, CancellationToken cancellationToken)
        {
            return client.PutAsync(requestUri, value, DefaultFormatter, cancellationToken);
        }

        /// <summary>
        ///     Sends a PUT request as an asynchronous operation to the specified Uri with the given <paramref name="value" />
        ///     serialized using MessagePack format.
        /// </summary>
        /// <remarks>
        ///     This method uses a default instance of <see cref="MessagePackMediaTypeFormatter" /> (
        ///     <see cref="ContractlessStandardResolver" />).
        /// </remarks>
        /// <typeparam name="T">The type of <paramref name="value" />.</typeparam>
        /// <param name="client">The client used to make the request.</param>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <param name="value">The value that will be placed in the request's entity body.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A task object representing the asynchronous operation.</returns>
        public static Task<HttpResponseMessage> PutAsMsgPackAsync<T>(
            this HttpClient client, string requestUri, T value, CancellationToken cancellationToken)
        {
            return client.PutAsync(requestUri, value, DefaultFormatter, cancellationToken);
        }
    }
}
