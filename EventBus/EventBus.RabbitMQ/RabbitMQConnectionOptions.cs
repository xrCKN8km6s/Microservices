namespace EventBus.RabbitMQ;

public class RabbitMQConnectionOptions
{
    public string HostName { get; set; } = "localhost";
    public string VirtualHost { get; set; } = "/";
    public string UserName { get; set; } = "guest";
    public string Password { get; set; } = "guest";
    public int ConnectRetryAttempts { get; set; } = 1;
}
