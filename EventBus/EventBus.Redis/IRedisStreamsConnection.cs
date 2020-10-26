using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace EventBus.Redis
{
    public interface IRedisStreamsConnection
    {
        void CreateConsumerGroup(string eventName);
        void DeleteConsumerGroup(string eventName);
        void PublishEvent(string eventId, string eventName, byte[] body);
        void Start(IReadOnlyCollection<string> streams, Func<string, string, string, Task> handler);
        void Stop();
    }
}
