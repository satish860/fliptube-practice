using AlterNats;

namespace FilpTube.History
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly INatsCommand natsCommand;

        public Worker(ILogger<Worker> logger, INatsCommand natsCommand)
        {
            _logger = logger;
            this.natsCommand = natsCommand;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await this.natsCommand.SubscribeAsync<string>("viewed", (path) =>
            {
                Console.WriteLine(path);
            });
        }
    }
}