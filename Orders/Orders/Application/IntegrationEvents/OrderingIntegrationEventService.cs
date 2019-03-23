using System;
using System.Data.Common;
using System.Threading.Tasks;
using EventBus.Abstractions;
using EventBus.Events;
using IntegrationEventLog.Services;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Orders.Infrastructure;

namespace Orders.Application.IntegrationEvents
{
    public class OrderingIntegrationEventService : IOrderingIntegrationEventService
    {
        private readonly MicroserviceContext _context;
        private readonly IEventBus _eventBus;
        private readonly ILogger<OrderingIntegrationEventService> _logger;
        private readonly IIntegrationEventLogService _eventLogService;

        public OrderingIntegrationEventService(
            [NotNull] Func<DbConnection, IIntegrationEventLogService> integrationEventLogServiceFactory,
            [NotNull] MicroserviceContext context,
            [NotNull] IEventBus eventBus,
            [NotNull] ILogger<OrderingIntegrationEventService> logger)
        {
            if (integrationEventLogServiceFactory == null)
            {
                throw new ArgumentNullException(nameof(integrationEventLogServiceFactory));
            }

            _eventLogService = integrationEventLogServiceFactory(context.Database.GetDbConnection());

            _context = context ?? throw new ArgumentNullException(nameof(context));
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task PublishEventsAsync()
        {
            var pendingIntegrationEvents = await _eventLogService.GetPendingAsync();

            foreach (var integrationEvent in pendingIntegrationEvents)
            {
                try
                {
                    _eventBus.Publish(integrationEvent.EventId, integrationEvent.EventName, integrationEvent.Content);
                    await _eventLogService.MarkAsPublishedAsync(integrationEvent.EventId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while processing IntegrationEvent {EventId}", integrationEvent.EventId);
                    await _eventLogService.MarkAsFailedAsync(integrationEvent.EventId);
                }
            }
        }

        public async Task SaveEventAsync(IntegrationEvent e)
        {
            await _eventLogService.AddAsync(e, _context.Database.CurrentTransaction.GetDbTransaction());
        }
    }
}