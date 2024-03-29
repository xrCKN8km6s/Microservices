using System.Diagnostics.CodeAnalysis;
using EventBus;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;

namespace IntegrationEventLog.Services;

public class IntegrationEventLogService : IIntegrationEventLogService
{
    private readonly ILogger<IntegrationEventLogService> _logger;
    private readonly IntegrationEventLogContext _context;

    public IntegrationEventLogService(IntegrationEventLogContext context, [NotNull] ILogger<IntegrationEventLogService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _context = context;
    }

    public async Task<IReadOnlyCollection<IntegrationEventLogItem>> GetPendingAsync(Guid transactionId)
    {
        await using var trans = await _context.Database.BeginTransactionAsync();

        var res = await _context.IntegrationEventLogItems
            .FromSqlInterpolated(
                $@"SELECT * FROM integration_event_logs WHERE ""State""={IntegrationEventState.NotPublished} AND
                                        ""TransactionId""={transactionId}
                                        ORDER BY ""CreatedDate"" FOR UPDATE SKIP LOCKED")
            .ToListAsync().ConfigureAwait(false);

        res.ForEach(item => item.State = IntegrationEventState.InProgress);

        await _context.SaveChangesAsync().ConfigureAwait(false);

        await trans.CommitAsync();

        return res;
    }

    public async Task AddAsync(IIntegrationEvent e, IDbContextTransaction transaction)
    {
        if (e == null)
        {
            throw new ArgumentNullException(nameof(e));
        }

        if (transaction == null)
        {
            throw new ArgumentNullException(nameof(transaction),
                "Transaction instance is required to ensure atomicity.");
        }

        var entry = IntegrationEventLogItem.Create(e, transaction.TransactionId);

        await _context.Database.UseTransactionAsync(transaction.GetDbTransaction());

        await _context.IntegrationEventLogItems.AddAsync(entry);

        await _context.SaveChangesAsync().ConfigureAwait(false);
    }

    public async Task MarkAsPublishedAsync(Guid eventId)
    {
        await SetStateAsync(eventId, IntegrationEventState.Published).ConfigureAwait(false);
    }

    public async Task MarkAsFailedAsync(Guid eventId)
    {
        await SetStateAsync(eventId, IntegrationEventState.Failed).ConfigureAwait(false);
    }

    private async Task SetStateAsync(Guid eventId, IntegrationEventState state)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var item = await _context.IntegrationEventLogItems
                .FromSqlInterpolated($"SELECT * FROM integration_event_logs WHERE \"EventId\"={eventId} FOR UPDATE")
                .SingleAsync()
                .ConfigureAwait(false);

            item.State = state;
            item.TimesSent++;

            _context.IntegrationEventLogItems.Update(item);

            await _context.SaveChangesAsync().ConfigureAwait(false);

            await transaction.CommitAsync();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Exception during integration event state update.");
        }
    }
}
