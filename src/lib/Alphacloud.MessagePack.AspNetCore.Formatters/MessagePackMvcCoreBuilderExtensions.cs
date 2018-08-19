namespace Alphacloud.MessagePack.AspNetCore.Formatters
{
    using System;
    using global::MessagePack.Resolvers;
    using JetBrains.Annotations;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Microsoft.Extensions.Options;


    /// <summary>
    ///     Configuration helper to use with MVC Core.
    /// </summary>
    [PublicAPI]
    public static class MessagePackMvcCoreBuilderExtensions
    {
        /// <summary>
        ///     Add MsgPack (application/x-message) input and output formatters.
        /// </summary>
        /// <exception cref="ArgumentNullException"><paramref name="builder" /> is <see langword="null" /></exception>
        public static IMvcCoreBuilder AddMessagePackFormatters(this IMvcCoreBuilder builder, Action<MessagePackFormatterOptions> setup = null)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            builder.Services.AddMessagePack(setup);
            return builder;
        }

        internal static void AddMessagePack(this IServiceCollection services, Action<MessagePackFormatterOptions> setup)
        {
            var options = new MessagePackFormatterOptions
            {
                FormatterResolver = ContractlessStandardResolver.Instance
            };

            setup?.Invoke(options);
            services.Add(ServiceDescriptor.Singleton(options));

            services.TryAddEnumerable(ServiceDescriptor.Transient<IConfigureOptions<MvcOptions>, MessagePackFormatterMvcOptionsSetup>());
        }
    }
}
