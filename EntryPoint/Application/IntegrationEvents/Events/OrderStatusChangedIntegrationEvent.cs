using EventBus.Events;

namespace EntryPoint.Application.IntegrationEvents.Events
{
    public class OrderStatusChangedIntegrationEvent : IntegrationEvent
    {
        public long OrderId { get; set; }

        public int OldOrderStatus { get; set; }

        public int NewOrderStatus { get; set; }
        public OrderStatusChangedIntegrationEvent(long orderId, int oldOrderStatus, int newOrderStatus)
        {
            OrderId = orderId;
            OldOrderStatus = oldOrderStatus;
            NewOrderStatus = newOrderStatus;
        }
    }
}