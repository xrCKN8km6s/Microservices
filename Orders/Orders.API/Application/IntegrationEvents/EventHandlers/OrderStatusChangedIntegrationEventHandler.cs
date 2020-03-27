using System;
using System.Threading.Tasks;
using EventBus;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using Orders.API.Application.IntegrationEvents.Events;

namespace Orders.API.Application.IntegrationEvents.EventHandlers
{
    public class OrderStatusChangedIntegrationEventHandler : IIntegrationEventHandler<OrderStatusChangedIntegrationEvent>
    {
        private readonly ILogger<OrderStatusChangedIntegrationEventHandler> _logger;

        public OrderStatusChangedIntegrationEventHandler([NotNull] ILogger<OrderStatusChangedIntegrationEventHandler> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        public Task Handle(OrderStatusChangedIntegrationEvent e)
        {
            _logger.LogInformation("Handling integration event {@event}", e);
            return Task.CompletedTask;
        }
    }
}