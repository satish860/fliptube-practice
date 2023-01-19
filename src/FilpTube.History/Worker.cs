using AlterNats;
using MongoDB.Bson;
using MongoDB.Driver;

namespace FilpTube.History
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly INatsCommand natsCommand;
        private readonly IMongoClient mongoClient;
        private readonly IConfiguration configuration;

        public Worker(ILogger<Worker> logger,
            INatsCommand natsCommand, 
            IMongoClient mongoClient,
            IConfiguration configuration)
        {
            _logger = logger;
            this.natsCommand = natsCommand;
            this.mongoClient = mongoClient;
            this.configuration = configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await this.natsCommand.SubscribeAsync<string>("viewed", (path) =>
            {
                var DBName = configuration["DBNAME"];
                var database = mongoClient.GetDatabase(DBName);
                var collection = database.GetCollection<VideoViewed>("videosviewed");
                collection.InsertOne(new VideoViewed { Id=ObjectId.GenerateNewId(), VideoPath= path });
            });
        }
    }
}