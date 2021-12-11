namespace EventBus.Redis;

public interface IRedisStreamsManager
{
    Task CreateConsumerGroup(string eventName);
    void DeleteConsumerGroup(string eventName);
    Task PublishEvent(string eventId, string eventName, byte[] body);
    void Start(IReadOnlyCollection<string> streams, Func<StreamsMessage, Task> handler);
    void Stop();
}
