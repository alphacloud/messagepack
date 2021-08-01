﻿namespace Tests.Functional.Setup
{
    using System;
    using System.Net.Http;
    using System.Net.Http.Formatting;
    using System.Net.Http.Headers;
    using Alphacloud.MessagePack.AspNetCore.Formatters;
    using Alphacloud.MessagePack.HttpFormatter;
    using MessagePack.Resolvers;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.TestHost;
    using NetCoreWebApi;


    public class TestServerSetup : IDisposable
    {
        public MediaTypeFormatterCollection Formatters { get; }
        public HttpClient Client { get; }
        public TestServer Server { get; }

        
        public TestServerSetup()
        {
            Server = new TestServer(Program.CreateWebHostBuilder(Array.Empty<string>())
#if NETCOREAPP3_1 || NET5_0 || NET6_0
                .UseTestServer()
#endif
                .UseStartup<Startup>()
                .UseSolutionRelativeContentRoot("Samples/NetCoreWebApi/")
            );
            Client = Server.CreateClient();
            Client.BaseAddress = Server.BaseAddress;
            Client.DefaultRequestHeaders.Accept.Add(MediaTypeWithQualityHeaderValue.Parse(MessagePackFormatterOptions.DefaultContentType));

            Formatters = new MediaTypeFormatterCollection(new MediaTypeFormatter[]
            {
                new MessagePackMediaTypeFormatter(ContractlessStandardResolver.Options, new[] {MessagePackMediaTypeFormatter.DefaultMediaType}),
                new BsonMediaTypeFormatter(),
                new JsonMediaTypeFormatter()
            });
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Server.Dispose();
            Client?.Dispose();
        }
    }
}
