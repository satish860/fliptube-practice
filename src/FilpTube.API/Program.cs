using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using Yarp.ReverseProxy.Forwarder;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables();
builder.Services.AddHttpClient();
var app = builder.Build();

app.MapGet("/video", async (HttpContext context, IHttpClientFactory httpClientFactory) =>
{
    var httpClient = httpClientFactory.CreateClient();
    var baseUrl = builder.Configuration["VIDEOSTORAGE"];
    httpClient.BaseAddress = new Uri(baseUrl);
    var responseMessage = await httpClient.GetAsync(
        "/video?videoname=SampleVideo_1280x720_1mb.mp4"
        ,HttpCompletionOption.ResponseHeadersRead
        ,context.RequestAborted);
    context.Response.StatusCode = (int)responseMessage.StatusCode;
    foreach (var header in responseMessage.Headers)
    {
        context.Response.Headers[header.Key] = header.Value.ToArray();
    }

    foreach (var header in responseMessage.Content.Headers)
    {
        context.Response.Headers[header.Key] = header.Value.ToArray();
    }
    await responseMessage.Content.CopyToAsync(context.Response.Body);
});

app.MapGet("/", () => "Hello World!");

app.Run();
