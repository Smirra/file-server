﻿// <auto-generated />
using System;
using Identity.Api.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Identity.Api.Data.Migrations
{
    [DbContext(typeof(IdentityApiDbContext))]
    [Migration("20231126180533_InitialCreate")]
    partial class InitialCreate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.HasPostgresExtension(modelBuilder, "uuid-ossp");
            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Identity.Api.Models.ResourceRole", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasDefaultValueSql("uuid_generate_v4()");

                    b.Property<bool>("CanDelete")
                        .HasColumnType("boolean");

                    b.Property<bool>("CanRead")
                        .HasColumnType("boolean");

                    b.Property<bool>("CanUpdate")
                        .HasColumnType("boolean");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("ResourceRoles");

                    b.HasData(
                        new
                        {
                            Id = new Guid("6003ee40-ae0f-4d0d-8fd9-09eba55411dc"),
                            CanDelete = true,
                            CanRead = true,
                            CanUpdate = true,
                            Name = "FileOwner"
                        },
                        new
                        {
                            Id = new Guid("0142b582-b743-4f76-a71b-cfec72e2e6c9"),
                            CanDelete = false,
                            CanRead = true,
                            CanUpdate = true,
                            Name = "FileEditor"
                        },
                        new
                        {
                            Id = new Guid("765bc68f-36bb-447f-9e66-69ae4b94946d"),
                            CanDelete = false,
                            CanRead = true,
                            CanUpdate = false,
                            Name = "FileViewer"
                        });
                });

            modelBuilder.Entity("Identity.Api.Models.Role", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasDefaultValueSql("uuid_generate_v4()");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex");

                    b.ToTable("Roles", (string)null);

                    b.HasData(
                        new
                        {
                            Id = new Guid("2815f0b8-e9c7-41eb-b4f2-f02016fcaa3b"),
                            Name = "Admin",
                            NormalizedName = "ADMIN"
                        },
                        new
                        {
                            Id = new Guid("a87780c7-c586-4a68-a783-2b0acef4b01b"),
                            Name = "User",
                            NormalizedName = "USER"
                        },
                        new
                        {
                            Id = new Guid("09c5ff95-1260-47d9-9b64-4b455b94f31e"),
                            Name = "Viewer",
                            NormalizedName = "VIEWER"
                        });
                });

            modelBuilder.Entity("Identity.Api.Models.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasDefaultValueSql("uuid_generate_v4()");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("integer");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("text");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("boolean");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("boolean");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("text");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("text");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("boolean");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("text");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("boolean");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex");

                    b.ToTable("Users", (string)null);

                    b.HasData(
                        new
                        {
                            Id = new Guid("35a69efb-83ac-462d-b54b-f636f58203a8"),
                            AccessFailedCount = 0,
                            ConcurrencyStamp = "a9590c63-ffa4-4580-99df-176edb232d92",
                            Email = "bob@bob.com",
                            EmailConfirmed = true,
                            FirstName = "Bobby",
                            LastName = "Bobson",
                            LockoutEnabled = false,
                            NormalizedEmail = "BOB@BOB.COM",
                            NormalizedUserName = "BOB",
                            PasswordHash = "AQAAAAIAAYagAAAAEFLBeCAlhlY4NTpeBEgXwyoH4mMT1eURc0md6QRO/7AHI+c6ULUKWT7CwqXJXM/RHw==",
                            PhoneNumberConfirmed = false,
                            SecurityStamp = "2d6d2a77-a1e8-4533-8dde-f3af261eb4fa",
                            TwoFactorEnabled = false,
                            UserName = "bob"
                        },
                        new
                        {
                            Id = new Guid("21c5d002-1fd7-41db-8437-ddabacfe5551"),
                            AccessFailedCount = 0,
                            ConcurrencyStamp = "3ccd3a95-1218-4e29-931f-becdcb0480cd",
                            Email = "alice@alice.com",
                            EmailConfirmed = true,
                            FirstName = "Alice",
                            LastName = "in Wonderland",
                            LockoutEnabled = false,
                            NormalizedEmail = "ALICE@ALICE.COM",
                            NormalizedUserName = "ALICE",
                            PasswordHash = "AQAAAAIAAYagAAAAEO5skWE6iaPdgCnIZSZMk8VB/YwK+nuM6z8nFZ+CFsHmj0rDML8+8Bqp5SMeKtwKaQ==",
                            PhoneNumberConfirmed = false,
                            SecurityStamp = "221e8b62-fb25-4b86-9fff-a3df0fbdd30b",
                            TwoFactorEnabled = false,
                            UserName = "alice"
                        },
                        new
                        {
                            Id = new Guid("be7780a1-8069-429a-a93c-66bd1f2231f4"),
                            AccessFailedCount = 0,
                            ConcurrencyStamp = "e15db919-c8b9-4ba8-bd87-421063ed34a2",
                            Email = "carlos@carlos.com",
                            EmailConfirmed = true,
                            FirstName = "Carlos",
                            LastName = "Carlinhos",
                            LockoutEnabled = false,
                            NormalizedEmail = "CARLOS@CARLOS.COM",
                            NormalizedUserName = "CARLOS",
                            PasswordHash = "AQAAAAIAAYagAAAAEO0h5DMrqzBmjY6hAApGzMkwjXBpK78RyqYmqbzSUZ21Wkoj0NJrK4UecpKQj3J5bg==",
                            PhoneNumberConfirmed = false,
                            SecurityStamp = "b37d5109-43c7-4701-8acb-d3cb2a7e75ba",
                            TwoFactorEnabled = false,
                            UserName = "carlos"
                        });
                });

            modelBuilder.Entity("Identity.Api.Models.UserFile", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("FileId")
                        .HasColumnType("uuid");

                    b.Property<bool>("CanShare")
                        .HasColumnType("boolean");

                    b.Property<Guid>("ResourceRoleId")
                        .HasColumnType("uuid");

                    b.HasKey("UserId", "FileId");

                    b.HasIndex("FileId");

                    b.HasIndex("ResourceRoleId");

                    b.ToTable("UserFiles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<System.Guid>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("text");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("text");

                    b.Property<Guid>("RoleId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("RoleClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<System.Guid>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("text");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("text");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("UserClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<System.Guid>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("text");

                    b.Property<string>("ProviderKey")
                        .HasColumnType("text");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("text");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("UserLogins", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<System.Guid>", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("RoleId")
                        .HasColumnType("uuid");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("UserRoles", (string)null);

                    b.HasData(
                        new
                        {
                            UserId = new Guid("35a69efb-83ac-462d-b54b-f636f58203a8"),
                            RoleId = new Guid("2815f0b8-e9c7-41eb-b4f2-f02016fcaa3b")
                        },
                        new
                        {
                            UserId = new Guid("21c5d002-1fd7-41db-8437-ddabacfe5551"),
                            RoleId = new Guid("a87780c7-c586-4a68-a783-2b0acef4b01b")
                        },
                        new
                        {
                            UserId = new Guid("be7780a1-8069-429a-a93c-66bd1f2231f4"),
                            RoleId = new Guid("09c5ff95-1260-47d9-9b64-4b455b94f31e")
                        });
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<System.Guid>", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<string>("Value")
                        .HasColumnType("text");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("UserTokens", (string)null);
                });

            modelBuilder.Entity("Identity.Api.Models.UserFile", b =>
                {
                    b.HasOne("Identity.Api.Models.ResourceRole", "ResourceRole")
                        .WithMany()
                        .HasForeignKey("ResourceRoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Identity.Api.Models.User", "User")
                        .WithMany("UserFiles")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ResourceRole");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<System.Guid>", b =>
                {
                    b.HasOne("Identity.Api.Models.Role", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<System.Guid>", b =>
                {
                    b.HasOne("Identity.Api.Models.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<System.Guid>", b =>
                {
                    b.HasOne("Identity.Api.Models.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<System.Guid>", b =>
                {
                    b.HasOne("Identity.Api.Models.Role", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Identity.Api.Models.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<System.Guid>", b =>
                {
                    b.HasOne("Identity.Api.Models.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Identity.Api.Models.User", b =>
                {
                    b.Navigation("UserFiles");
                });
#pragma warning restore 612, 618
        }
    }
}
