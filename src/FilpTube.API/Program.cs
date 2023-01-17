using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/video", () =>
{
    var filename = "SampleVideo_1280x720_1mb.mp4";
    //Build the File Path.  
    string path = Path.Combine(builder.Environment.WebRootPath, "files/") + filename;  // the video file is in the wwwroot/files folder  

    var filestream = System.IO.File.OpenRead(path);
    return Results.File(filestream, contentType: "video/mp4", enableRangeProcessing: true);
});

app.MapGet("/", () => "Hello World!");

app.Run();
