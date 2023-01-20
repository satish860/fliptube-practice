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
builder.Configuration.AddEnvironmentVariables();
builder.Services.AddSingleton<IMongoClient, MongoClient>(s =>
{
    var uri = s.GetRequiredService<IConfiguration>()["DBHOST"];
    Console.WriteLine(uri);
    return new MongoClient(uri);
});
var natsHostName = builder.Configuration["NATSHOST"];
builder.Services.AddNats(poolSize: 4, 
    configureOptions: opt => 
    opt with { Url = $"{natsHostName}:4222"});
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
        $"/video/{video.Path}"
        ,HttpCompletionOption.ResponseHeadersRead
        ,context.RequestAborted);
    var fileStream = await responseMessage.Content.ReadAsStreamAsync();
    return Results.File(fileStream, responseMessage.Content.Headers?.ContentType?.MediaType,enableRangeProcessing: true);
});

app.MapGet("/", () => "Hello World!");

app.Run();
