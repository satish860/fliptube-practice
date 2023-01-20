using Amazon.Extensions.NETCore.Setup;
using Amazon.Runtime;
using Amazon.S3;
using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace FilpTube.VideoStorage
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var app = builder.Build();

            app.MapGet("/video/{videoName}", async (string videoName,IConfiguration configuration) =>
            {
                var Config = new AmazonS3Config
                {
                    ServiceURL = configuration["DOCTL:ServiceURL"],
                    ForcePathStyle= false,
                };
                var awsAccessKey = configuration["AWS_ACCESS_KEY"];
                var awsSecret = configuration["AWS_SECRET"];
                var cred = new BasicAWSCredentials(awsAccessKey, awsSecret);
                AmazonS3Client amazonS3Client = new AmazonS3Client(cred,Config);
                var bucketName = configuration["DOCTL:BucketName"];
                var response = await amazonS3Client.GetObjectAsync(bucketName, videoName);
                return Results.File(response.ResponseStream, contentType: "video/mp4", enableRangeProcessing: true);
            });

            app.MapGet("/", () => "Hello World!");


            app.Run();
        }
    }
}