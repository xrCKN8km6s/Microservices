using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;
using EventBus;

namespace IntegrationEventLog.Services
{
    public interface IIntegrationEventLogService
    {
        /// <summary>
        /// Retrieves pending events and sets state to InProgress.
        /// </summary>
        /// <returns></returns>
        Task<IReadOnlyCollection<IntegrationEventLogItem>> GetPendingAsync();

        Task AddAsync(IntegrationEvent e, DbTransaction transaction);

        Task MarkAsInProgressAsync(Guid eventId);

        Task MarkAsPublishedAsync(Guid eventId);

        Task MarkAsFailedAsync(Guid eventId);
    }
}