using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Raft.Example.Services
{
    public class RaftService : BackgroundService
    {
        private readonly RaftNode node;

        public RaftService(RaftNode node)
        {
            this.node = node;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.Factory.StartNew(() => node.Run(stoppingToken),
                stoppingToken,
                TaskCreationOptions.LongRunning,
                TaskScheduler.Default
            ).Unwrap();
        }
    }
}