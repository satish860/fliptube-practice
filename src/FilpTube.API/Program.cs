using AlterNats;
using FilpTube.API;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using Yarp.ReverseProxy.Forwarder;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<IMongoClient, MongoClient>(s =>
{
    var uri = s.GetRequiredService<IConfiguration>()["DBHOST"];
    return new MongoClient(uri);
});
var natsHostName = builder.Configuration["NATSHOST"];
builder.Services.AddNats(poolSize: 4, 
    configureOptions: opt => 
    opt with { Url = $"Nats:4222"});
builder.Configuration.AddEnvironmentVariables();
builder.Services.AddHttpClient();
var app = builder.Build();

app.MapGet("/video", async ([FromQuery] string videoid,HttpContext context, 
    IHttpClientFactory httpClientFactory,
    IMongoClient mongoClient,
    INatsCommand natsCommand) =>
{
    var DBName = builder.Configuration["DBNAME"];
    var database = mongoClient.GetDatabase(DBName);
    var collection = database.GetCollection<Video>("videos");
    var video = collection.Find(document => document.Id == ObjectId.Parse(videoid)).FirstOrDefault();
   
    await natsCommand.PublishAsync<string>("viewed", video.Path);
    var httpClient = httpClientFactory.CreateClient();
    var baseUrl = builder.Configuration["VIDEOSTORAGE"];
    httpClient.BaseAddress = new Uri(baseUrl);
    var responseMessage = await httpClient.GetAsync(
        $"/video?videoname={video.Path}"
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
