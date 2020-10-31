namespace EventBus.Redis
{
    public readonly struct StreamsMessage
    {
        public StreamsMessage(string eventName, string messageId, string content)
        {
            EventName = eventName;
            MessageId = messageId;
            Content = content;
        }

        public string EventName { get; }
        public string MessageId { get; }
        public string Content { get; }
    }
}
