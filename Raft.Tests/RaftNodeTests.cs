using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Raft.DTOs;

namespace Raft.Tests
{
    [TestClass]
    public class RaftNodeTests
    {
        #region RPC Handler Tests

        [TestMethod]
        public async Task AppendEntries_IssuesHeartbeat()
        {
            var log = new Mock<ILogger<RaftNode>>();
            var channel = Channel.CreateBounded<bool>(new BoundedChannelOptions(1)
            {
                FullMode = BoundedChannelFullMode.DropWrite,
                SingleReader = true,
                SingleWriter = true
            });

            var node = new RaftNode(log.Object, channel);

            await node.AppendEntries(new AppendEntriesRequest());

            if (!channel.Reader.TryRead(out var _))
            {
                Assert.Fail("No heartbeat was available to read");
            }
        }

        #endregion
    }
}