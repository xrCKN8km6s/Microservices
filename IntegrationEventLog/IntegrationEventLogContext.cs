using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IntegrationEventLog
{
    public class IntegrationEventLogContext : DbContext
    {
        public DbSet<IntegrationEventLogItem> IntegrationEventLogItems { get; set; }

        public IntegrationEventLogContext(DbContextOptions<IntegrationEventLogContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new IntegrationEventLogItemTypeConfiguration());
        }
    }

    public class IntegrationEventLogItemTypeConfiguration : IEntityTypeConfiguration<IntegrationEventLogItem>
    {
        public void Configure(EntityTypeBuilder<IntegrationEventLogItem> builder)
        {
            builder.ToTable("integration_event_logs");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Name).IsRequired();

            builder.Property(p => p.CreatedDate).IsRequired();

            builder.Property(p => p.TimesSent).IsRequired();

            builder.Property(p => p.Content).HasColumnType("json").IsRequired();

            builder.Property(p => p.State).IsRequired();
        }
    }

    internal class IntegrationEventLogContextDesignFactory : IDesignTimeDbContextFactory<IntegrationEventLogContext>
    {
        public IntegrationEventLogContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<IntegrationEventLogContext>()
                .UseNpgsql("Host=localhost;Database=MicroserviceDb;Username=db_user;Password=db_pass");

            return new IntegrationEventLogContext(optionsBuilder.Options);
        }
    }
}