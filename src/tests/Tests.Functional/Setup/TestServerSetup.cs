namespace Tests.Functional.Setup
{
    using System;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using Alphacloud.MessagePack.AspNetCore.Formatters;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.TestHost;
    using NetCoreWebApi;


    public class TestServerSetup : IDisposable
    {
        public HttpClient Client { get; }
        public TestServer Server { get; }

        /// <inheritdoc />
        public TestServerSetup()
        {
            Server = new TestServer(Program.CreateWebHostBuilder(Array.Empty<string>())
                    .UseStartup<Startup>()
                .UseSolutionRelativeContentRoot("Samples/NetCoreWebApi/")
            );
            Client = Server.CreateClient();
            Client.BaseAddress = Server.BaseAddress;
            Client.DefaultRequestHeaders.Accept.Add(MediaTypeWithQualityHeaderValue.Parse(MessagePackFormatterOptions.DefaultContentType));
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Server?.Dispose();
            Client?.Dispose();
        }
    }
}
