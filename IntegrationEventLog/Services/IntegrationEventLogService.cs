using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;
using EventBus.Events;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace IntegrationEventLog.Services
{
    public class IntegrationEventLogService : IIntegrationEventLogService
    {
        private readonly ILogger<IntegrationEventLogService> _logger;
        private readonly IntegrationEventLogContext _context;

        public IntegrationEventLogService(DbConnection connection, [NotNull] ILogger<IntegrationEventLogService> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            var builder = new DbContextOptionsBuilder<IntegrationEventLogContext>().UseNpgsql(connection);
            _context = new IntegrationEventLogContext(builder.Options);
        }

        public async Task<IReadOnlyCollection<IntegrationEventLogItem>> GetPendingAsync()
        {
            using (var trans = _context.Database.BeginTransaction())
            {
                var res = await _context.IntegrationEventLogItems
                    .FromSql(
                        "SELECT * FROM integration_event_logs WHERE \"State\"={0} ORDER BY \"CreatedDate\" FOR UPDATE SKIP LOCKED",
                        IntegrationEventState.NotPublished)
                    .ToListAsync();

                res.ForEach(item => item.State = IntegrationEventState.InProgress);

                await _context.SaveChangesAsync();

                trans.Commit();

                return res;
            }
        }

        public async Task AddAsync(IntegrationEvent e, [NotNull] DbTransaction transaction)
        {
            if (transaction == null)
            {
                throw new ArgumentNullException(nameof(transaction),
                    "Transaction instance is required to ensure atomicity.");
            }

            var entry = IntegrationEventLogItem.Create(e);

            _context.Database.UseTransaction(transaction);

            await _context.IntegrationEventLogItems.AddAsync(entry);

            await _context.SaveChangesAsync();
        }

        public async Task MarkAsInProgressAsync(Guid eventId)
        {
            await SetState(eventId, IntegrationEventState.InProgress);
        }

        public async Task MarkAsPublishedAsync(Guid eventId)
        {
            await SetState(eventId, IntegrationEventState.Published);
        }

        public async Task MarkAsFailedAsync(Guid eventId)
        {
            await SetState(eventId, IntegrationEventState.Failed);
        }

        private async Task SetState(Guid eventId, IntegrationEventState state)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var item = await _context.IntegrationEventLogItems
                        .FromSql("SELECT * FROM integration_event_logs WHERE \"EventId\"={0} FOR UPDATE", eventId).SingleAsync();

                    item.State = state;
                    item.TimesSent++;

                    _context.IntegrationEventLogItems.Update(item);

                    await _context.SaveChangesAsync();

                    transaction.Commit();
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Ex");
                }
            }
        }
    }
}