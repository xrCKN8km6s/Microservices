using System;
using System.Data.Common;
using System.Reflection;
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

        public OrderingIntegrationEventService(MicroserviceContext context,
            Func<DbConnection, IIntegrationEventLogService> integrationEventLogServiceFactory, [NotNull] IEventBus eventBus)
        {
            _context = context;
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
            _eventLogService = integrationEventLogServiceFactory(context.Database.GetDbConnection());
        }

        public async Task PublishEventsAsync()
        {
            var pending = await _eventLogService.GetPendingAsync();

            foreach (var e in pending)
            {
                try
                {
                    var assembly = Assembly.GetEntryAssembly();

                    var type = assembly.GetType(e.Name);
                    
                    _eventBus.Publish(e.GetIntegrationEvent(type));
                    await _eventLogService.MarkAsPublishedAsync(e.Id);
                }
                catch (Exception ex)
                {
                    await _eventLogService.MarkAsFailedAsync(e.Id);
                }
            }
        }

        public async Task AddEventAsync(IntegrationEvent e)
        {
            await _eventLogService.AddAsync(e, _context.GetCurrentTransaction.GetDbTransaction());
        }
    }
}