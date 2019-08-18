using System;
using Newtonsoft.Json;

namespace EventBus.Events
{
    public class IntegrationEvent
    {
        public Guid Id { get; }

        public DateTime CreationDate { get; }

        public IntegrationEvent()
        {
            Id = Guid.NewGuid();
            CreationDate = DateTime.UtcNow;
        }

        [JsonConstructor]
        public IntegrationEvent(Guid id, DateTime creationDate)
        {
            Id = id;
            CreationDate = creationDate;
        }
    }
}