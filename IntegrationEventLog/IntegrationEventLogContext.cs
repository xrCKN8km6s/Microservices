using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IntegrationEventLog
{
    public class IntegrationEventLogContext : DbContext
    {
        public DbSet<IntegrationEventLogItem> IntegrationEventLogItems { get; set; }

        public IntegrationEventLogContext(DbContextOptions<IntegrationEventLogContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            if (modelBuilder is null)
            {
                throw new System.ArgumentNullException(nameof(modelBuilder));
            }

            modelBuilder.ApplyConfiguration(new IntegrationEventLogItemTypeConfiguration());
        }
    }

    public class IntegrationEventLogItemTypeConfiguration : IEntityTypeConfiguration<IntegrationEventLogItem>
    {
        public void Configure(EntityTypeBuilder<IntegrationEventLogItem> builder)
        {
            if (builder is null)
            {
                throw new System.ArgumentNullException(nameof(builder));
            }

            builder.ToTable("integration_event_logs");

            builder.HasKey(p => p.EventId);

            builder.Property(p => p.EventName).IsRequired();

            builder.Property(p => p.CreatedDate).IsRequired();

            builder.Property(p => p.TimesSent).IsRequired();

            builder.Property(p => p.Content).HasColumnType("json").IsRequired();

            builder.Property(p => p.State).IsRequired();
        }
    }
}