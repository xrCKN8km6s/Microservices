using EventBus.Events;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;

namespace IntegrationEventLog.Services
{
    public interface IIntegrationEventLogService
    {
        Task<IReadOnlyCollection<IntegrationEventLogItem>> GetPendingAsync();

        Task AddAsync(IntegrationEvent e, DbTransaction transaction);

        Task MarkAsPublishedAsync(Guid eventId);

        Task MarkAsFailedAsync(Guid eventId);
    }
}