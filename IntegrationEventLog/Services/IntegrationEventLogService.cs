using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;
using EventBus.Events;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace IntegrationEventLog.Services
{
    public class IntegrationEventLogService : IIntegrationEventLogService
    {
        private readonly IntegrationEventLogContext _context;

        public IntegrationEventLogService(DbConnection connection)
        {
            var builder = new DbContextOptionsBuilder<IntegrationEventLogContext>().UseNpgsql(connection);
            _context = new IntegrationEventLogContext(builder.Options);
        }

        public async Task<IReadOnlyCollection<IntegrationEventLogItem>> GetPendingAsync()
        {
            var res = await _context.IntegrationEventLogItems
                .FromSql("SELECT * FROM integration_event_logs WHERE \"State\"={0} FOR UPDATE SKIP LOCKED", IntegrationEventLogItem.IntegrationEventState.NotPublished)
                .ToArrayAsync();
            return res;
        }

        public async Task AddAsync(IntegrationEvent e, [NotNull] DbTransaction transaction)
        {
            if (transaction == null)
            {
                throw new ArgumentNullException(nameof(transaction), "Transaction instance is required to ensure atomicity.");
            }

            var entry = new IntegrationEventLogItem(e);

            _context.Database.UseTransaction(transaction);

            await _context.IntegrationEventLogItems.AddAsync(entry);

            await _context.SaveChangesAsync();
        }

        public async Task MarkAsPublishedAsync(Guid eventId)
        {
            var item = await _context.IntegrationEventLogItems
                .FromSql("SELECT * FROM integration_event_logs WHERE \"Id\"={0} FOR UPDATE", eventId).SingleAsync();

            item.State = IntegrationEventLogItem.IntegrationEventState.Published;
            item.TimesSent++;

            _context.IntegrationEventLogItems.Update(item);

            await _context.SaveChangesAsync();
        }

        public Task MarkAsFailedAsync(Guid eventId)
        {
            throw new NotImplementedException();
        }
    }
}