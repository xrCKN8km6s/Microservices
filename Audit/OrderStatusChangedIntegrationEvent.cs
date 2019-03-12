using EventBus.Events;
using JetBrains.Annotations;

namespace Audit
{
    [UsedImplicitly]
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