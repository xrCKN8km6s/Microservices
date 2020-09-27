namespace EventBus.Redis
{
    public class RedisEventBusOptions
    {
        public string Configuration { get; set; }
        public string ConsumerGroupName { get; set; }
        public string ConsumerName { get; set; }
        public int BatchPerGroupSize { get; set; }
    }
}
