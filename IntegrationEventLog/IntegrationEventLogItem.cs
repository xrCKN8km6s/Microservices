using System;
using EventBus.Events;
using Newtonsoft.Json;

namespace IntegrationEventLog
{
    public class IntegrationEventLogItem
    {
        public enum IntegrationEventState
        {
            NotPublished,
            InProgress,
            Published,
            Failed
        }

        public Guid Id { get; private set; }

        public string Name { get; private set; }

        public DateTime CreatedDate { get; private set; }

        public int TimesSent { get; set; }

        public string Content { get; private set; }

        public IntegrationEventState State { get; set; }

        private IntegrationEventLogItem()
        {
        }

        public IntegrationEventLogItem(IntegrationEvent e)
        {
            Id = e.Id;
            CreatedDate = e.CreationDate;
            Name = e.GetType().FullName;
            State = IntegrationEventState.NotPublished;
            TimesSent = 0;
            Content = JsonConvert.SerializeObject(e);
        }

        public IntegrationEvent GetIntegrationEvent(Type type) =>
            JsonConvert.DeserializeObject(Content, type) as IntegrationEvent;
    }
}