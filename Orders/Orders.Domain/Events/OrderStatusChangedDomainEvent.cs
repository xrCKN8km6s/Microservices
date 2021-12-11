using MediatR;

namespace Orders.Domain.Events;

public class OrderStatusChangedDomainEvent : INotification
{
    public long Id { get; }
    public int OldStatus { get; }
    public int NewStatus { get; }

    public OrderStatusChangedDomainEvent(long id, int oldStatus, int newStatus)
    {
        Id = id;
        OldStatus = oldStatus;
        NewStatus = newStatus;
    }
}
