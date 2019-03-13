using EntryPoint.Domain;
using EntryPoint.Domain.Aggregates.Order;
using JetBrains.Annotations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace EntryPoint.Infrastructure
{
    public class MicroserviceContext : DbContext, IUnitOfWork
    {
        private readonly IMediator _mediator;

        public DbSet<Order> Orders { get; set; }

        public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
        {
            await _mediator.DispatchDomainEventsAsync(this);

            await SaveChangesAsync(cancellationToken);

            return true;
        }

        public MicroserviceContext(DbContextOptions<MicroserviceContext> options, IMediator mediator) : base(options)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        #region Transaction

        private IDbContextTransaction _currentTransaction;

        public IDbContextTransaction GetCurrentTransaction => _currentTransaction;

        public bool HasActiveTransaction => _currentTransaction != null;

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            _currentTransaction = await Database.BeginTransactionAsync(IsolationLevel.ReadCommitted);

            return _currentTransaction;
        }

        public async Task CommitTransactionAsync(IDbContextTransaction transaction)
        {
            if (transaction == null) throw new ArgumentNullException(nameof(transaction));
            if (transaction != _currentTransaction) throw new InvalidOperationException($"Transaction {transaction.TransactionId} is not current");

            try
            {
                await SaveChangesAsync();
                transaction.Commit();
            }
            catch
            {
                RollbackTransaction();
                throw;
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }

        public void RollbackTransaction()
        {
            try
            {
                _currentTransaction?.Rollback();
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }

        #endregion


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ForNpgsqlUseSequenceHiLo();

            modelBuilder.ApplyConfiguration(new OrderEntityTypeConfiguration());
        }
    }

    public class OrderEntityTypeConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.ToTable("orders");

            builder.ForNpgsqlHasIndex(p => p.Id);

            builder.Ignore(p => p.DomainEvents);

            builder.Property<DateTime>("CreationDateTime").IsRequired();

            builder.Property<int>("OrderStatus").IsRequired();

            builder.Property<string>("Name").IsRequired();
        }
    }

    [UsedImplicitly]
    internal class MicroserviceContextDesignFactory : IDesignTimeDbContextFactory<MicroserviceContext>
    {
        public MicroserviceContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<MicroserviceContext>().UseNpgsql("Host=localhost;Database=MicroserviceDb;Username=db_user;Password=db_pass");

            return new MicroserviceContext(optionsBuilder.Options, new DesignTimeMediator());
        }

        private class DesignTimeMediator : IMediator
        {
            public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = new CancellationToken())
            {
                return Task.FromResult<TResponse>(default);
            }

            public Task Publish(object notification, CancellationToken cancellationToken = new CancellationToken())
            {
                return Task.CompletedTask;
            }

            public Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = new CancellationToken()) where TNotification : INotification
            {
                return Task.CompletedTask;
            }
        }
    }
}
