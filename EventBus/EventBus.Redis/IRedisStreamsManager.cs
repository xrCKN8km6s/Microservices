using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EventBus.Redis
{
    public interface IRedisStreamsManager
    {
        void CreateConsumerGroup(string eventName);
        void DeleteConsumerGroup(string eventName);
        void PublishEvent(string eventId, string eventName, byte[] body);
        void Start(IReadOnlyCollection<string> streams, Func<string, string, string, Task> handler);
        void Stop();
    }
}
