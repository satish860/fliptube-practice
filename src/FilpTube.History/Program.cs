using AlterNats;

namespace FilpTube.History
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IHost host = Host.CreateDefaultBuilder(args)
                .ConfigureServices(services =>
                {
                    
                    var natsHostName = "Nats";
                    services.AddNats(poolSize: 4,
                                     configureOptions: opt =>
                                     opt with { Url = $"{natsHostName}:4222", 
                                     ConnectOptions = ConnectOptions.Default with { Name = "MyClient" } 
                                    });
                    services.AddHostedService<Worker>();
                })
                .Build();

            host.Run();
        }
    }
}