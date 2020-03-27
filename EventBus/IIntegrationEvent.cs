using System;

namespace EventBus
{
    public interface IIntegrationEvent
    {
        public Guid Id { get; }

        public DateTimeOffset CreationDate { get; }
    }
}