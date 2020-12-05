namespace Alphacloud.MessagePack.AspNetCore.Formatters
{
    using System;
    using global::MessagePack.Resolvers;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Microsoft.Extensions.Options;


    /// <summary>
    ///     MessagePack services configurator.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        ///     Add MsgPack (application/x-message) input and output formatters.
        /// </summary>
        /// <exception cref="ArgumentNullException"><paramref name="services" /> is <see langword="null" /></exception>
        public static void AddMessagePack(this IServiceCollection services, Action<MessagePackFormatterOptions>? setup = null)
        {
            services.Configure<MessagePackFormatterOptions>(o =>
            {
                if (o.FormatterResolver == null) o.FormatterResolver = ContractlessStandardResolver.Instance;
                setup?.Invoke(o);
            });

            services.TryAddEnumerable(ServiceDescriptor.Transient<IConfigureOptions<MvcOptions>, MessagePackFormatterMvcOptionsSetup>());
        }
    }
}
