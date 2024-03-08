namespace Tests.Functional;

using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using Alphacloud.MessagePack.AspNetCore.Formatters;
using Alphacloud.MessagePack.HttpFormatter;
using FluentAssertions;
using MessagePack.Resolvers;
using Microsoft.AspNetCore.Mvc.Testing;
using NetCoreWebApi;
using NetCoreWebApi.Models;
using Xunit;


public class MessagePackFormatterTests : IClassFixture<WebApplicationFactory<Program>>
{
    readonly WebApplicationFactory<Program> _factory;
    MediaTypeFormatterCollection _formatters { get; }
    HttpClient _client { get; }

    public MessagePackFormatterTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory ?? throw new ArgumentNullException(nameof(factory));

        _client = _factory.CreateClient();
        _client.DefaultRequestHeaders.Accept.Add(MediaTypeWithQualityHeaderValue.Parse(MessagePackFormatterOptions.DefaultContentType));

        _formatters = new MediaTypeFormatterCollection(new MediaTypeFormatter[]
        {
            new MessagePackMediaTypeFormatter(ContractlessStandardResolver.Options, new[] {MessagePackMediaTypeFormatter.DefaultMediaType}),
            new BsonMediaTypeFormatter(),
            new JsonMediaTypeFormatter()
        });

    }

    static async Task<T> ReadData<T>(HttpResponseMessage response)
    {
        response.EnsureSuccessStatusCode();
        var stream = await response.Content.ReadAsStreamAsync();
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
        var response = await _client.GetAsync(new Uri($"/api/values/format/20.{format}", UriKind.Relative));
        response.EnsureSuccessStatusCode();
        response.Content.Headers.ContentType!.MediaType.Should().Be(mediaType);
    }

    [Fact]
    public async Task CanGet()
    {
        var response = await _client.GetAsync(new Uri("/api/values", UriKind.Relative));
        response.EnsureSuccessStatusCode();
        var res = await ReadData<IEnumerable<TestModel>>(response);
        res.Should().OnlyContain(x => x.Id == 1 || x.Id == 2);
    }

    [Fact]
    public async Task CanGetUsingWebApiClient()
    {
        using var response = await _client.GetAsync(new Uri("/api/values", UriKind.Relative));
        response.EnsureSuccessStatusCode();
        var res = await response.Content.ReadAsAsync<IEnumerable<TestModel>>(_formatters)
            ;
        res.Should().OnlyContain(x => x.Id == 1 || x.Id == 2);
    }


    [Fact]
    public async Task CanGetJson()
    {
        using var req = new HttpRequestMessage(HttpMethod.Get, new Uri("/api/values", UriKind.Relative));
        req.Headers.Accept.Clear();
        req.Headers.Accept.Add(MediaTypeWithQualityHeaderValue.Parse("application/json"));
        using var response = await _client.SendAsync(req);
        response.EnsureSuccessStatusCode();
        response.Content.Headers.ContentType!.MediaType.Should().Be("application/json");
    }

    [Fact]
    public async Task CanPost()
    {
        var model = new TestModel(10);

        using var req = new HttpRequestMessage(HttpMethod.Post, new Uri("/api/values", UriKind.Relative));
        req.Content = new ByteArrayContent(MessagePackSerializer.Serialize(model, ContractlessStandardResolver.Options));
        req.Content.Headers.ContentType = MediaTypeHeaderValue.Parse(MessagePackFormatterOptions.DefaultContentType);

        using var response = await _client.SendAsync(req);
        var res = await ReadData<TestModel>(response);
        res.Should().BeEquivalentTo(model);
    }

    [Fact]
    public async Task CanPostUriUsingWebApiClient()
    {
        var testModel = new TestModel(20);
        var uri = new Uri("/api/values", UriKind.Relative);

        using var response = await _client.PostAsMsgPackAsync(uri, testModel, CancellationToken.None);
        response.EnsureSuccessStatusCode();
        var res = await response.Content.ReadAsAsync<TestModel>(_formatters, CancellationToken.None);
        res.Should().BeEquivalentTo(testModel);
    }

    [Fact]
    public async Task CanPutUriUsingWebApiClient()
    {
        var testModel = new TestModel(20);
        var uri = new Uri("/api/values", UriKind.Relative);

        using var response = await _client.PutAsMsgPackAsync(uri, testModel, CancellationToken.None);
        response.EnsureSuccessStatusCode();
        var res = await response.Content.ReadAsAsync<TestModel>(_formatters, CancellationToken.None);
        res.Should().BeEquivalentTo(testModel);
    }

    [Fact]
    public async Task CanPostStringUsingWebApiClient()
    {
        var testModel = new TestModel(20);

        using var response = await _client.PostAsMsgPackAsync("/api/values", testModel, CancellationToken.None);
        response.EnsureSuccessStatusCode();
        var res = await response.Content.ReadAsMsgPackAsync<TestModel>();
        res.Should().BeEquivalentTo(testModel);
    }

    [Fact]
    public async Task CanPutStringUsingWebApiClient()
    {
        var testModel = new TestModel(20);

        using var response = await _client.PutAsMsgPackAsync("/api/values", testModel, CancellationToken.None);
        response.EnsureSuccessStatusCode();
        var res = await response.Content.ReadAsMsgPackAsync<TestModel>();
        res.Should().BeEquivalentTo(testModel);
    }

}
