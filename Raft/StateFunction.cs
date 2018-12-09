using System.Threading;
using System.Threading.Tasks;

namespace Raft
{
    public delegate Task<StateFunction> StateFunction(CancellationToken token);
}