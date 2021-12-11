namespace EventBus.RabbitMQ;

public class RabbitMQEventBusOptions
{
    public string ExchangeName { get; set; }
    public string QueueName { get; set; }
    public int PublishRetryAttempts { get; set; } = 1;
}
