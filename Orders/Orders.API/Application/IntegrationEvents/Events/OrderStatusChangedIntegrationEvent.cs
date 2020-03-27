using EventBus;
using System;

namespace Orders.API.Application.IntegrationEvents.Events
{
    public class OrderStatusChangedIntegrationEvent : IIntegrationEvent
    {
        public Guid Id { get; set; }

        public DateTimeOffset CreationDate { get; set; }

        public long OrderId { get; set; }

        public int OldOrderStatus { get; set; }

        public int NewOrderStatus { get; set; }

        public OrderStatusChangedIntegrationEvent(long orderId, int oldOrderStatus, int newOrderStatus)
        {
            Id = Guid.NewGuid();
            CreationDate = DateTimeOffset.UtcNow;
            OrderId = orderId;
            OldOrderStatus = oldOrderStatus;
            NewOrderStatus = newOrderStatus;
        }
    }
}