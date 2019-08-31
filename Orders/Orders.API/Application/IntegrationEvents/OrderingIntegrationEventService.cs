using System;
using System.Threading.Tasks;
using EventBus.Abstractions;
using EventBus.Events;
using IntegrationEventLog.Services;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Orders.Infrastructure;

namespace Orders.Application.IntegrationEvents
{
    public class OrderingIntegrationEventService : IOrderingIntegrationEventService
    {
        private readonly OrdersContext _context;
        private readonly IEventBus _eventBus;
        private readonly ILogger<OrderingIntegrationEventService> _logger;
        private readonly IIntegrationEventLogService _integrationEventLogService;

        public OrderingIntegrationEventService(
            [NotNull] IIntegrationEventLogService integrationEventLogService,
            [NotNull] OrdersContext context,
            [NotNull] IEventBus eventBus,
            [NotNull] ILogger<OrderingIntegrationEventService> logger)
        {
            _integrationEventLogService = integrationEventLogService ?? throw new ArgumentNullException(nameof(integrationEventLogService));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task PublishEventsAsync()
        {
            var pendingIntegrationEvents = await _integrationEventLogService.GetPendingAsync();

            foreach (var integrationEvent in pendingIntegrationEvents)
            {
                try
                {
                    _eventBus.Publish(integrationEvent.EventId, integrationEvent.EventName, integrationEvent.Content);
                    await _integrationEventLogService.MarkAsPublishedAsync(integrationEvent.EventId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while processing IntegrationEvent {EventId}", integrationEvent.EventId);
                    await _integrationEventLogService.MarkAsFailedAsync(integrationEvent.EventId);
                }
            }
        }

        public async Task SaveEventAsync(IntegrationEvent e)
        {
            await _integrationEventLogService.AddAsync(e, _context.Database.CurrentTransaction.GetDbTransaction());
        }
    }
}