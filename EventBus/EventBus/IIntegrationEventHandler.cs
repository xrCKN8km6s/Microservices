namespace EventBus;

public interface IIntegrationEventHandler<in TIntegrationEvent> where TIntegrationEvent : IIntegrationEvent
{
    Task Handle(TIntegrationEvent e);
}
