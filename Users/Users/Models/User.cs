using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Users.Models
{
    public class User
    {
        public long Id { get; set; }

        public string Sub { get; set; }

        public List<UserRole> UserRoles { get; set; }

        public bool IsActive { get; set; }

        public User()
        {
            UserRoles = new List<UserRole>();
        }
    }

    public class UserRole
    {
        public long UserId { get; set; }

        public User User { get; set; }

        public long RoleId { get; set; }

        public Role Role { get; set; }
    }

    public class Role
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public bool IsGlobal { get; set; }

        public bool IsActive { get; set; }

        public List<UserRole> UserRoles { get; set; }

        public List<PermissionRole> PermissionRoles { get; set; }

        public Role()
        {
            UserRoles = new List<UserRole>();
            PermissionRoles = new List<PermissionRole>();
        }
    }

    public class PermissionRole
    {
        public long PermissionId { get; set; }

        public Permission Permission { get; set; }

        public long RoleId { get; set; }

        public Role Role { get; set; }
    }

    public class Permission
    {
        public long Id { get; set; }
        public string Name { get; set; }

        public List<PermissionRole> PermissionRoles { get; set; }

        public Permission()
        {
            PermissionRoles = new List<PermissionRole>();
        }
    }

    public class Permission1 : IEquatable<Permission1>
    {
        public long Id { get; private set; }

        public string Description { get; private set; }

        private  Permission1() { }

        private Permission1(long id, string description)
        {
            Id = id;
            Description = description;
        }

        public static Permission1 ViewOrders = new Permission1(1, "View orders");

        public static Permission1 EditOrders = new Permission1(1, "Edit orders");

        public bool Equals(Permission1 other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(Permission1)) return false;
            return Equals((Permission1) obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public static bool operator ==(Permission1 left, Permission1 right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Permission1 left, Permission1 right)
        {
            return !Equals(left, right);
        }

        public static string NameFromId(long id)
        {
            return null;
        }

        public static long IdFromName(string name)
        {
            return 0;
        }
    }

    public class UsersContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public DbSet<Role> Roles { get; set; }

        public DbSet<UserRole> UserRoles { get; set; }

        public DbSet<Permission> Permissions { get; set; }

        public UsersContext(DbContextOptions<UsersContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UserEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new UserRoleEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new PermissionEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new PermissionRoleEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new RoleEntityTypeConfiguration());
        }
    }

    public class UserEntityTypeConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("users");
            builder.HasKey(p => p.Id);
            builder.HasIndex(p => p.Id);
            builder.Property(p => p.Id).HasColumnName("id").IsRequired();
            builder.Property(p => p.Sub).HasColumnName("sub").IsRequired();
            builder.Property(p => p.IsActive).HasColumnName("is_active").IsRequired();
        }
    }

    public class UserRoleEntityTypeConfiguration : IEntityTypeConfiguration<UserRole>
    {
        public void Configure(EntityTypeBuilder<UserRole> builder)
        {
            builder.ToTable("user_roles");
            builder.HasKey(k => new {k.RoleId, k.UserId});
            builder.HasIndex(k => new {k.RoleId, k.UserId}).IsUnique();
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

    public class PermissionEntityTypeConfiguration : IEntityTypeConfiguration<Permission>
    {
        public void Configure(EntityTypeBuilder<Permission> builder)
        {
            builder.ToTable("permissions");
            builder.HasKey(p => p.Id);
            builder.HasIndex(p => p.Id);
            builder.Property(p => p.Id).HasColumnName("id").IsRequired();
            builder.Property(p => p.Name).HasColumnName("name").IsRequired();
        }
    }

    public class PermissionRoleEntityTypeConfiguration : IEntityTypeConfiguration<PermissionRole>
    {
        public void Configure(EntityTypeBuilder<PermissionRole> builder)
        {
            builder.ToTable("permission_roles");
            builder.HasKey(k => new {k.RoleId, k.PermissionId});
            builder.HasIndex(k => new {k.RoleId, k.PermissionId}).IsUnique();
            builder.Property(p => p.RoleId).HasColumnName("role_id").IsRequired();
            builder.Property(p => p.PermissionId).HasColumnName("permission_id").IsRequired();
            builder.HasOne(m => m.Permission)
                .WithMany(m => m.PermissionRoles)
                .HasForeignKey(k => k.PermissionId);
            builder.HasOne(o => o.Role)
                .WithMany(m => m.PermissionRoles)
                .HasForeignKey(k => k.RoleId);
        }
    }

    public class RoleEntityTypeConfiguration : IEntityTypeConfiguration<Role>
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
        }
    }

    internal class MicroserviceContextDesignFactory : IDesignTimeDbContextFactory<UsersContext>
    {
        public UsersContext CreateDbContext(string[] args)
        {
            var optionsBuilder =
                new DbContextOptionsBuilder<UsersContext>().UseNpgsql(
                    "Host=localhost;Database=Users;Username=db_user;Password=db_pass");

            return new UsersContext(optionsBuilder.Options);
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