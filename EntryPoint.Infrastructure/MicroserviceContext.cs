using EntryPoint.Domain;
using EntryPoint.Domain.Aggregates.Order;
using JetBrains.Annotations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace EntryPoint.Infrastructure
{
    public class MicroserviceContext : DbContext, IUnitOfWork
    {
        private readonly IMediator _mediator;

        public DbSet<Order> Orders { get; set; }

        public MicroserviceContext(DbContextOptions<MicroserviceContext> options, IMediator mediator) : base(options)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        #region Transaction

        public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
        {
            await _mediator.DispatchDomainEventsAsync(this);

            await SaveChangesAsync(cancellationToken);

            return true;
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
            var optionsBuilder =
                new DbContextOptionsBuilder<MicroserviceContext>().UseNpgsql(
                    "Host=localhost;Database=MicroserviceDb;Username=db_user;Password=db_pass");

            return new MicroserviceContext(optionsBuilder.Options, new DesignTimeMediator());
        }

        private class DesignTimeMediator : IMediator
        {
            public Task<TResponse> Send<TResponse>(IRequest<TResponse> request,
                CancellationToken cancellationToken = new CancellationToken())
            {
                return Task.FromResult<TResponse>(default);
            }

            public Task Publish(object notification, CancellationToken cancellationToken = new CancellationToken())
            {
                return Task.CompletedTask;
            }

            public Task Publish<TNotification>(TNotification notification,
                CancellationToken cancellationToken = new CancellationToken()) where TNotification : INotification
            {
                return Task.CompletedTask;
            }
        }
    }
}