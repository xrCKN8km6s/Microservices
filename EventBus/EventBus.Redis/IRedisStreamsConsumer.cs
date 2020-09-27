using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace EventBus.Redis
{
    public interface IRedisStreamsConsumer
    {
        void Start(IReadOnlyCollection<string> streams, Func<string, StreamEntry, Task> handler);
        void Stop();
    }
}
