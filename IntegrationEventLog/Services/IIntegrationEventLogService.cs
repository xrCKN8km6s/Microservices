using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EventBus;
using Microsoft.EntityFrameworkCore.Storage;

namespace IntegrationEventLog.Services
{
    public interface IIntegrationEventLogService
    {
        /// <summary>
        /// Retrieves pending events for current transaction and sets state to InProgress.
        /// </summary>
        /// <returns></returns>
        Task<IReadOnlyCollection<IntegrationEventLogItem>> GetPendingAsync(Guid transactionId);

        Task AddAsync(IntegrationEvent e, IDbContextTransaction transaction);

        Task MarkAsInProgressAsync(Guid eventId);

        Task MarkAsPublishedAsync(Guid eventId);

        Task MarkAsFailedAsync(Guid eventId);
    }
}