using System;
using EventBus;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace IntegrationEventLog
{
    public class IntegrationEventLogItem
    {
        public Guid EventId { get; private set; }

        public string EventName { get; private set; }

        public DateTime CreatedDate { get; private set; }

        public int TimesSent { get; set; }

        public string Content { get; private set; }

        public IntegrationEventState State { get; set; }

        //Used by EF
        [UsedImplicitly]
        private IntegrationEventLogItem()
        {
        }

        public static IntegrationEventLogItem Create(IntegrationEvent e)
        {
            return new IntegrationEventLogItem
            {
                EventId = e.Id,
                CreatedDate = e.CreationDate,
                EventName = e.GetType().Name,
                State = IntegrationEventState.NotPublished,
                TimesSent = 0,
                Content = JsonConvert.SerializeObject(e)
            };
        }
    }
}