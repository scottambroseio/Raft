using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Raft.DTOs;

namespace Raft
{
    public class RaftNode
    {
        private readonly Channel<bool> heartbeatChannel;
        private readonly ILogger<RaftNode> log;

        public RaftNode(ILogger<RaftNode> log, Channel<bool> heartbeatChannel)
        {
            this.heartbeatChannel = heartbeatChannel;
            this.log = log;
        }

        public async Task Run(CancellationToken token)
        {
            log.LogInformation("Starting state machine");

            StateFunction stateFunction = Follower;

            while (stateFunction != null)
            {
                stateFunction = await stateFunction(token);
            }

            log.LogInformation("State machine execution terminated");
        }

        #region RPC Handlers

        public async Task<AppendEntriesResult> AppendEntries(AppendEntriesRequest rpc)
        {
            await heartbeatChannel.Writer.WriteAsync(true);

            return new AppendEntriesResult();
        }

        #endregion

        #region State Functions

        private async Task<StateFunction> Follower(CancellationToken token)
        {
            log.LogInformation("Became Follower");

            while (true)
            {
                var heartbeat = heartbeatChannel.Reader.ReadAsync(token);
                var timeout = Task.Delay(TimeSpan.FromSeconds(1), token);

                var result = await Task.WhenAny(heartbeat.AsTask(), timeout);

                if (result.IsCanceled)
                {
                    log.LogInformation("Received cancellation signal");
                    return null;
                }

                if (result != timeout)
                {
                    log.LogInformation("Received heartbeat");
                    continue;
                }

                log.LogInformation("Transitioning to Candidate");
                return null;
            }
        }

        #endregion
    }
}