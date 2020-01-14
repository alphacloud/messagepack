#if NETCOREAPP2_1 || NETCOREAPP2_2
namespace Alphacloud.MessagePack.AspNetCore.Formatters
{
    using System;
    using JetBrains.Annotations;
    using Microsoft.Extensions.DependencyInjection;


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
        [Obsolete("Use IServiceCollection.AddMassagePack")]
        public static IMvcCoreBuilder AddMessagePackFormatters(this IMvcCoreBuilder builder, Action<MessagePackFormatterOptions> setup = null)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            builder.Services.AddMessagePack(setup);

            return builder;
        }
    }
}

#endif
