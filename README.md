# MessagePack Formatters for ASP.NET Core MVC and HttpClient

Allows to use [MessagePack format](http://msgpack.org/) with ASP.NET Core MVC. If multiple formatters are configured,
content negotiation is used to determine which formatter to use.

This library leverages [MessagePack-CSharp](https://github.com/neuecc/MessagePack-CSharp) library by Yoshifumi Kawai (a.k.a. neuecc).

## Build status

||Stable|Pre-release|
|:--:|:--:|:--:|
| Build                                   | [![Master](https://ci.appveyor.com/api/projects/status/jvcg5663lannifb9/branch/master?svg=true)](https://ci.appveyor.com/project/shatl/messagepack/branch/master) | [![PreRelease](https://ci.appveyor.com/api/projects/status/jvcg5663lannifb9?svg=true)](https://ci.appveyor.com/project/shatl/messagepack) |
| NuGet ASP.NET Core MVC formatters       | [![NuGet](https://img.shields.io/nuget/v/Alphacloud.MessagePack.AspNetCore.Formatters.svg)](https://www.nuget.org/packages/Alphacloud.MessagePack.AspNetCore.Formatters) | [![NuGet](https://img.shields.io/nuget/vpre/Alphacloud.MessagePack.AspNetCore.Formatters.svg)](https://www.nuget.org/packages/Alphacloud.MessagePack.AspNetCore.Formatters/absoluteLatest) |
| NuGet MediaTypeFormatter for HttpClient | [![NuGet](https://img.shields.io/nuget/v/Alphacloud.MessagePack.HttpFormatter.svg)](https://www.nuget.org/packages/Alphacloud.MessagePack.HttpFormatter) | [![NuGet](https://img.shields.io/nuget/vpre/Alphacloud.MessagePack.HttpFormatter.svg)](https://www.nuget.org/packages/Alphacloud.MessagePack.HttpFormatter/absoluteLatest) |


## Packages
* [ASP.NET Core MVC formatters](#aspnet-core-mvc-formatters)
* [MediaTypeFormatter for HttpClient](#mediatypeformatter-for-httpclient)


# ASP.NET Core MVC formatters

## Installation

```
Install-Packagge Alphacloud.MessagePack.AspNetCore.Formatters
```


## Features

* Input formatter (decode MessagePack payload).
* Output formatter (encode MessagePack payload).
* Source link support (source code on demand in debugger).


## Usage

### Default configuration

Default configuration uses `application/x-msgpack` as media typeidentificator, `msgpack` as file name extension and `ContractlessStandardResolver`.

Add `services.AddMessagePack();` call to your `Startup.cs / ConfigureServices`
```
public void ConfigureServices(IServiceCollection services)
{
    services.AddMessagePack();
    // ...
}
```

#### MVC Core (ASP.NET Core 2.x)

When using minimal MVC configuration (e.g. in WebAPI service) only base services are added by default.
You are responsible for configuring each of the service you are going to use.

**Note:** order of formatters is important - in the example below Json will be default serializer,
unless MVC is configured to reject requests with unsupported media type.

```
public void ConfigureServices(IServiceCollection services)
{
  services.AddMvcCore(options =>
    {
        // ...
    })
    .AddDataAnnotations()
    .AddJsonOptions(options =>
    {
        options.SerializerSettings.Culture = CultureInfo.InvariantCulture;
        options.SerializerSettings.Formatting = Formatting.None;
    })
    .AddFormatterMappings()
    .AddJsonFormatters()
    .AddXmlSerializerFormatters()
    .AddMsgPackFormatters()
    ;
}
```

### Custom configuration

Default configuration can be changed by providing callback to `AddMessagePack` method.

Available settings:
* `MediaTypes` - allows to specify additional media handled by MessagePack formatters. Default is `application/x-msgpack`.
* `FormatterResolver` - allows to customize serialization, see [MsgPack object serialization](https://github.com/neuecc/MessagePack-CSharp/blob/master/README.md#object-serialization) for details.
* `FileExtensions` - allows to specify reponse format URL mapping. Default is `msgpack`. See sample project or
[Microsoft Docs](https://docs.microsoft.com/en-us/aspnet/core/web-api/advanced/formatting?view=aspnetcore-2.2#response-format-url-mappings) for more information.
* `Compression` - allows to specify LZ4 compression for messages.
* `UseOldSpecification` - write message using old specification compatibility mode. Reading will support both old and new specifications. See [Message Pack format specification](https://github.com/msgpack/msgpack/blob/master/spec-old.md).
* `OmitAssemblyVersion` - don't write assembly version and public key token when using typeless formatter.
* `AllowAssemblyVersionMismatch` - allows assembly version mistmatch when loading types during deserialization.

```
services.AddMessagePack(opt =>
  {
      opt.MediaTypes.Clear();
      opt.MediaTypes.Add("application/x-messagepack");
      opt.FileExtensions.Add("messagepack");
      opt.FormatterResolver = MessagePack.Resolvers.ContractlessStandardResolverAllowPrivate.Instance;
  })

```


# MediaTypeFormatter for HttpClient

[Microsoft.AspNet.WebApi.Client](https://www.nuget.org/packages/Microsoft.AspNet.WebApi.Client/) package contains adds support for formatting and content negotiation to System.Net.Http.
It allows to add custom content serializers by extending `MediaTypeFormatter` class.


## Installation
```
Install-Package Alphacloud.MessagePack.HttpFormatter
```

## Usage

### Sending request

Library contains two extension methods `PostAsMsgPackAsync` and `PutAsMsgPackAsync`. To serialize payload `ContractlessStandardResolver` is used, which gives a JSON-like experience.

```
var response = await httpClient.PostAsMsgPackAsync(uri, payload, cancellationToken).ConfigureAwait(false);

```

### Reading response
To deserialize Msgpack response, formatter must be added to `formatters` collection passed to `ReadAsAsync` extension method.

This is **recommeded** method as it enables content negotiation.
```
var res = await response.Content.ReadAsAsync<TestModel>(_formatters, CancellationToken.None);
```

if you are expecting only MsgPack content, use `MsgPackHttpContentExtensions.ReadMsgPackAsAsync()` for `HttpContent`.

# Samples

Sample application is located at `src/samples/NetCoreWebApi`.
Sample Postman requests can be found at `src/samples/MessagePack.postman_collection.json`. To post MessagePack content, use `SingleModel.msgpack` located under `src/samples`.


# References

* MessagePack format https://msgpack.org/index.html
* MessagePack-CSharp https://github.com/neuecc/MessagePack-CSharp
* AddMVC vs AddMvcCore
  * https://www.stevejgordon.co.uk/aspnetcore-anatomy-deep-dive-index
  * https://offering.solutions/blog/articles/2017/02/07/difference-between-addmvc-addmvcore/
* Content negotiation in ASP.NET Core MVC https://code-maze.com/content-negotiation-dotnet-core/
* Microsoft.AspNet.WebApi.Client https://www.nuget.org/packages/Microsoft.AspNet.WebApi.Client/
* Source Link - https://github.com/ctaggart/SourceLink
* Postman https://www.getpostman.com/downloads/
