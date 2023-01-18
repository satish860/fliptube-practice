using Amazon.Extensions.NETCore.Setup;
using Amazon.Runtime;
using Amazon.S3;
using System.IO;

namespace FilpTube.VideoStorage
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            // builder.Services.AddDefaultAWSOptions(builder.Configuration.GetAWSOptions());
            var app = builder.Build();

            app.MapGet("/video", async (IConfiguration configuration) =>
            {
                var Config = new AmazonS3Config
                {
                    ServiceURL = "https://sgp1.digitaloceanspaces.com",
                    ForcePathStyle= false,
                };
                var awsAccessKey = configuration["AWS_ACCESS_KEY"];
                var awsSecret = configuration["AWS_SECRET"];
                var cred = new BasicAWSCredentials(awsAccessKey, awsSecret);
                AmazonS3Client amazonS3Client = new AmazonS3Client(cred,Config);
                var response = await amazonS3Client.GetObjectAsync("bmdk", "SampleVideo_1280x720_1mb.mp4");
                return Results.File(response.ResponseStream, contentType: "video/mp4", enableRangeProcessing: true);
            });

            app.MapGet("/", () => "Hello World!");


            app.Run();
        }
    }
}