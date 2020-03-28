namespace EventBus.RabbitMQ.AspNetCore
{
    public class RabbitMQOptions
    {
        public int ConnectRetryAttempts { get; set; } = 1;
        public string HostName { get; set; } = "localhost";
        public string VirtualHost { get; set; } = "/";
        public string UserName { get; set; } = "guest";
        public string Password { get; set; } = "guest";
        public string ExchangeName { get; set; }
        public string QueueName { get; set; }
        public int PublishRetryAttempts { get; set; } = 1;
    }
}
