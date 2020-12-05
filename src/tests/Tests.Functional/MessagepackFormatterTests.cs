namespace Tests.Functional
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading;
    using System.Threading.Tasks;
    using Alphacloud.MessagePack.AspNetCore.Formatters;
    using Alphacloud.MessagePack.HttpFormatter;
    using FluentAssertions;
    using MessagePack;
    using MessagePack.Resolvers;
    using NetCoreWebApi.Models;
    using Setup;
    using Xunit;


    public class MessagePackFormatterTests : IClassFixture<TestServerSetup>
    {
        readonly TestServerSetup _setup;

        public MessagePackFormatterTests(TestServerSetup setup)
        {
            _setup = setup ?? throw new ArgumentNullException(nameof(setup));
        }

        static async Task<T> ReadData<T>(HttpResponseMessage response)
        {
            response.EnsureSuccessStatusCode();
            var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
            var res = MessagePackSerializer.Deserialize<T>(
                stream,
                ContractlessStandardResolver.Options
            );
            return res;
        }

        [Theory]
        [InlineData("json", "application/json")]
        [InlineData("msgpack", "application/x-msgpack")]
        public async Task CanRequestFormat(string format, string mediaType)
        {
            var response = await _setup.Client.GetAsync(new Uri($"/api/values/format/20.{format}", UriKind.Relative));
            response.EnsureSuccessStatusCode();
            response.Content.Headers.ContentType!.MediaType.Should().Be(mediaType);
        }

        [Fact]
        public async Task CanGet()
        {
            var response = await _setup.Client.GetAsync(new Uri("/api/values", UriKind.Relative));
            response.EnsureSuccessStatusCode();
            var res = await ReadData<IEnumerable<TestModel>>(response);
            res.Should().OnlyContain(x => x.Id == 1 || x.Id == 2);
        }

        [Fact]
        public async Task CanGetUsingWebApiClient()
        {
            using var response = await _setup.Client.GetAsync(new Uri("/api/values", UriKind.Relative));
            response.EnsureSuccessStatusCode();
            var res = await response.Content.ReadAsAsync<IEnumerable<TestModel>>(_setup.Formatters)
                .ConfigureAwait(false);
            res.Should().OnlyContain(x => x.Id == 1 || x.Id == 2);
        }


        [Fact]
        public async Task CanGetJson()
        {
            using var req = new HttpRequestMessage(HttpMethod.Get, new Uri("/api/values", UriKind.Relative));
            req.Headers.Accept.Clear();
            req.Headers.Accept.Add(MediaTypeWithQualityHeaderValue.Parse("application/json"));
            using var response = await _setup.Client.SendAsync(req).ConfigureAwait(false);
            response!.EnsureSuccessStatusCode();
            response.Content.Headers.ContentType!.MediaType.Should().Be("application/json");
        }

        [Fact]
        public async Task CanPost()
        {
            var model = new TestModel(10);

            using var req = new HttpRequestMessage(HttpMethod.Post, new Uri("/api/values", UriKind.Relative));
            req.Content = new ByteArrayContent(MessagePackSerializer.Serialize(model, ContractlessStandardResolver.Options));
            req.Content.Headers.ContentType = MediaTypeHeaderValue.Parse(MessagePackFormatterOptions.DefaultContentType);

            using var response = await _setup.Client.SendAsync(req).ConfigureAwait(false);
            var res = await ReadData<TestModel>(response).ConfigureAwait(false);
            res.Should().BeEquivalentTo(model);
        }

        [Fact]
        public async Task CanPostUsingWebApiClient()
        {
            var testModel = new TestModel(20);
            var uri = new Uri("/api/values", UriKind.Relative);

            using var response = await _setup.Client.PostAsMsgPackAsync(uri, testModel, CancellationToken.None).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            var res = await response.Content.ReadAsAsync<TestModel>(_setup.Formatters, CancellationToken.None);
            res.Should().BeEquivalentTo(testModel);
        }
    }
}
