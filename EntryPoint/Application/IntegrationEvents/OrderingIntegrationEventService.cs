using System;
using System.Data.Common;
using System.Threading.Tasks;
using EntryPoint.Infrastructure;
using EventBus.Abstractions;
using EventBus.Events;
using IntegrationEventLog.Services;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace EntryPoint.Application.IntegrationEvents
{
    public class OrderingIntegrationEventService : IOrderingIntegrationEventService
    {
        private readonly MicroserviceContext _context;
        private readonly IEventBus _eventBus;
        private readonly IIntegrationEventLogService _eventLogService;

        public OrderingIntegrationEventService(
            [NotNull] Func<DbConnection, IIntegrationEventLogService> integrationEventLogServiceFactory,
            [NotNull] MicroserviceContext context,
            [NotNull] IEventBus eventBus)
        {
            if (integrationEventLogServiceFactory == null)
            {
                throw new ArgumentNullException(nameof(integrationEventLogServiceFactory));
            }

            _context = context ?? throw new ArgumentNullException(nameof(context));
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
            _eventLogService = integrationEventLogServiceFactory(context.Database.GetDbConnection());
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
                catch
                {
                    await _eventLogService.MarkAsFailedAsync(integrationEvent.EventId);
                }
            }
        }

        public async Task SaveEventAsync(IntegrationEvent e)
        {
            await _eventLogService.AddAsync(e, _context.GetCurrentTransaction.GetDbTransaction());
        }
    }
}