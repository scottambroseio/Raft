using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Raft.Example.Services;

namespace Raft.Example
{
    public class Program
    {
        public static async Task Main()
        {
            var host = new HostBuilder()
                .ConfigureLogging(logging => logging.AddConsole())
                .ConfigureServices(services =>
                {
                    services.Configure<ConsoleLifetimeOptions>(options => options.SuppressStatusMessages = true);

                    var heartbeatChannel = Channel.CreateBounded<bool>(new BoundedChannelOptions(1)
                    {
                        FullMode = BoundedChannelFullMode.DropWrite,
                        SingleReader = true,
                        SingleWriter = true
                    });

                    services.AddSingleton(provider => new RaftNode(provider.GetService<ILogger<RaftNode>>(), heartbeatChannel));
                    services.AddHostedService<RaftService>();
                });

            await host.RunConsoleAsync();
        }
    }
}