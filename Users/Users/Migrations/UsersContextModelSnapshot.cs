﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Users.Models;

namespace Users.Migrations
{
    [DbContext(typeof(UsersContext))]
    partial class UsersContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .HasAnnotation("ProductVersion", "2.2.3-servicing-35854")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("Users.Models.Permission", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnName("name");

                    b.HasKey("Id");

                    b.HasIndex("Id");

                    b.ToTable("permissions");
                });

            modelBuilder.Entity("Users.Models.PermissionRole", b =>
                {
                    b.Property<long>("RoleId")
                        .HasColumnName("role_id");

                    b.Property<long>("PermissionId")
                        .HasColumnName("permission_id");

                    b.HasKey("RoleId", "PermissionId");

                    b.HasIndex("PermissionId");

                    b.HasIndex("RoleId", "PermissionId")
                        .IsUnique();

                    b.ToTable("permission_roles");
                });

            modelBuilder.Entity("Users.Models.Role", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id");

                    b.Property<bool>("IsActive")
                        .HasColumnName("is_active");

                    b.Property<bool>("IsGlobal")
                        .HasColumnName("is_global");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnName("name");

                    b.HasKey("Id");

                    b.HasIndex("Id");

                    b.ToTable("roles");
                });

            modelBuilder.Entity("Users.Models.User", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id");

                    b.Property<bool>("IsActive")
                        .HasColumnName("is_active");

                    b.Property<string>("Sub")
                        .IsRequired()
                        .HasColumnName("sub");

                    b.HasKey("Id");

                    b.HasIndex("Id");

                    b.ToTable("users");
                });

            modelBuilder.Entity("Users.Models.UserRole", b =>
                {
                    b.Property<long>("RoleId")
                        .HasColumnName("role_id");

                    b.Property<long>("UserId")
                        .HasColumnName("user_id");

                    b.HasKey("RoleId", "UserId");

                    b.HasIndex("UserId");

                    b.HasIndex("RoleId", "UserId")
                        .IsUnique();

                    b.ToTable("user_roles");
                });

            modelBuilder.Entity("Users.Models.PermissionRole", b =>
                {
                    b.HasOne("Users.Models.Permission", "Permission")
                        .WithMany("PermissionRoles")
                        .HasForeignKey("PermissionId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Users.Models.Role", "Role")
                        .WithMany("PermissionRoles")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Users.Models.UserRole", b =>
                {
                    b.HasOne("Users.Models.Role", "Role")
                        .WithMany("UserRoles")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Users.Models.User", "User")
                        .WithMany("UserRoles")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
