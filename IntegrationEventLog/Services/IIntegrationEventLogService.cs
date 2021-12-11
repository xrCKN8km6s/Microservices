using EventBus;
using Microsoft.EntityFrameworkCore.Storage;

namespace IntegrationEventLog.Services;

public interface IIntegrationEventLogService
{
    /// <summary>
    /// Retrieves pending events for current transaction and sets state to InProgress.
    /// </summary>
    /// <returns></returns>
    Task<IReadOnlyCollection<IntegrationEventLogItem>> GetPendingAsync(Guid transactionId);

    Task AddAsync(IIntegrationEvent e, IDbContextTransaction transaction);

    Task MarkAsPublishedAsync(Guid eventId);

    Task MarkAsFailedAsync(Guid eventId);
}
