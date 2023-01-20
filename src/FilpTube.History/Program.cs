using AlterNats;
using MongoDB.Driver;

namespace FilpTube.History
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IHostBuilder host = Host.CreateDefaultBuilder(args);
            host.ConfigureAppConfiguration(c =>
            {
                c.AddEnvironmentVariables();
            });

            host.ConfigureServices((hostContext, services) =>
            {

                services.AddSingleton<IMongoClient, MongoClient>(s =>
                {
                    var uri = s.GetRequiredService<IConfiguration>()["DBHOST"];
                    return new MongoClient(uri);
                });
                var natsHostName = hostContext.Configuration["NATSHOST"];
                services.AddNats(poolSize: 4,
                                 configureOptions: opt =>
                                 opt with { Url = $"{natsHostName}:4222",
                                     ConnectOptions = ConnectOptions.Default with { Name = "MyClient" }
                                 });
                services.AddHostedService<Worker>();
            });

            var app = host.Build();

            app.Run();
        }
    }
}