using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Orders.Domain;
using Orders.Domain.Aggregates.Order;

namespace Orders.Infrastructure
{
    public class OrdersContext : DbContext, IUnitOfWork
    {
        private readonly IMediator _mediator;

        public DbSet<Order> Orders { get; set; }

        public OrdersContext(DbContextOptions<OrdersContext> options, IMediator mediator) : base(options)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        #region Transaction

        public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
        {
            await _mediator.DispatchDomainEventsAsync(this).ConfigureAwait(false);

            await SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            return true;
        }

        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new OrderEntityTypeConfiguration());
        }
    }

    public class OrderEntityTypeConfiguration : IEntityTypeConfiguration<Order>
    {
        //https://docs.microsoft.com/en-us/ef/core/what-is-new/ef-core-3.0/breaking-changes#field-only-property-names-should-match-the-field-name
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.ToTable("orders");

            builder.HasKey(p => p.Id);

            builder.HasIndex(p => p.Id);

            builder.Property(p => p.Id).IsRequired();

            builder.Property<DateTime>("_creationDateTime").HasColumnName("CreationDateTime").IsRequired();

            builder.Property<int>("_orderStatus").HasColumnName("OrderStatus").IsRequired();

            builder.Property<string>("_name").HasColumnName("Name").IsRequired();

            builder.Ignore(p => p.DomainEvents);
        }
    }
}