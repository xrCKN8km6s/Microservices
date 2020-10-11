using System;
using EventBus;
using Newtonsoft.Json;

namespace IntegrationEventLog
{
    public class IntegrationEventLogItem
    {
        public Guid EventId { get; private set; }

        public string EventName { get; private set; }

        public DateTimeOffset CreatedDate { get; private set; }

        public int TimesSent { get; set; }

        public string Content { get; private set; }

        public IntegrationEventState State { get; set; }

        public Guid TransactionId { get; private set; }

        //Used by EF
        private IntegrationEventLogItem()
        {
        }

        public static IntegrationEventLogItem Create(IIntegrationEvent e, Guid transactionId)
        {
            if (e == null) throw new ArgumentNullException(nameof(e));

            return new IntegrationEventLogItem
            {
                EventId = e.Id,
                CreatedDate = e.CreationDate,
                EventName = e.GetType().Name,
                State = IntegrationEventState.NotPublished,
                TimesSent = 0,
                Content = JsonConvert.SerializeObject(e),
                TransactionId = transactionId
            };
        }
    }
}
