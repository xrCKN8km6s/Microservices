using Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Users.Infrastructure
{
    public class UsersContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public DbSet<Role> Roles { get; set; }

        public DbSet<UserRole> UserRoles { get; set; }

        public DbSet<PermissionRole> PermissionRoles { get; set; }

        public UsersContext(DbContextOptions<UsersContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UserEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new UserRoleEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new PermissionRoleEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new RoleEntityTypeConfiguration());
        }
    }

    internal class UserEntityTypeConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("users");
            builder.HasKey(p => p.Id);
            builder.HasIndex(p => p.Id);
            builder.Property(p => p.Id).HasColumnName("id").IsRequired();
            builder.Property(p => p.Sub).HasColumnName("sub").IsRequired();
            builder.Property(p => p.Name).HasColumnName("name").IsRequired();
            builder.Property(p => p.Email).HasColumnName("email").IsRequired();
            builder.Property(p => p.IsActive).HasColumnName("is_active").IsRequired();

            builder.HasQueryFilter(user => user.IsActive);
        }
    }

    internal class UserRoleEntityTypeConfiguration : IEntityTypeConfiguration<UserRole>
    {
        public void Configure(EntityTypeBuilder<UserRole> builder)
        {
            builder.ToTable("user_roles");
            builder.HasKey(k => new { k.RoleId, k.UserId });
            builder.HasIndex(k => new { k.RoleId, k.UserId }).IsUnique();
            builder.Property(p => p.RoleId).HasColumnName("role_id").IsRequired();
            builder.Property(p => p.UserId).HasColumnName("user_id").IsRequired();
            builder
                .HasOne(m => m.User)
                .WithMany(m => m.UserRoles)
                .HasForeignKey(k => k.UserId);
            builder
                .HasOne(o => o.Role)
                .WithMany(m => m.UserRoles)
                .HasForeignKey(k => k.RoleId);
        }
    }

    internal class PermissionRoleEntityTypeConfiguration : IEntityTypeConfiguration<PermissionRole>
    {
        public void Configure(EntityTypeBuilder<PermissionRole> builder)
        {
            builder.ToTable("permission_roles");
            builder.HasKey(k => new {k.RoleId, k.Permission});
            builder.HasIndex(k => new {k.RoleId, k.Permission}).IsUnique();
            builder.Property(p => p.RoleId).HasColumnName("role_id").IsRequired();
            builder.Property(p => p.Permission).HasConversion(v => v.Id, v => Permission.Parse(v))
                .HasColumnName("permission").IsRequired();
            builder.HasOne(o => o.Role)
                .WithMany(m => m.PermissionRoles)
                .HasForeignKey(k => k.RoleId);
        }
    }

    internal class RoleEntityTypeConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.ToTable("roles");
            builder.HasKey(k => k.Id);
            builder.HasIndex(p => p.Id);
            builder.Property(p => p.Id).HasColumnName("id").IsRequired();
            builder.Property(p => p.Name).HasColumnName("name").IsRequired();
            builder.Property(p => p.IsGlobal).HasColumnName("is_global").IsRequired();
            builder.Property(p => p.IsActive).HasColumnName("is_active").IsRequired();

            builder.HasQueryFilter(role => role.IsActive);
        }
    }
}