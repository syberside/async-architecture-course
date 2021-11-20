﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using aTES.Billing.DataAccess;

namespace aTES.Billing.Migrations
{
    [DbContext(typeof(BillingDbContext))]
    partial class BillingDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.10")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            modelBuilder.Entity("aTES.Billing.DataAccess.DbTask", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<Guid?>("AssignedUserId")
                        .HasColumnType("uuid")
                        .HasColumnName("assigned_user_id");

                    b.Property<int>("BirdInCageCost")
                        .HasColumnType("integer");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("description");

                    b.Property<string>("JiraId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("MilletInABowlCost")
                        .HasColumnType("integer");

                    b.Property<string>("PublicId")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("public-id");

                    b.HasKey("Id");

                    b.HasIndex("AssignedUserId");

                    b.ToTable("tasks");
                });

            modelBuilder.Entity("aTES.Billing.DataAccess.DbTransactionLogRecord", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("created_at");

                    b.Property<long>("Credit")
                        .HasColumnType("bigint")
                        .HasColumnName("credit");

                    b.Property<long>("Debit")
                        .HasColumnType("bigint")
                        .HasColumnName("debit");

                    b.Property<string>("Details")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("details");

                    b.Property<Guid>("OwnerId")
                        .HasColumnType("uuid")
                        .HasColumnName("owner_id");

                    b.HasKey("Id");

                    b.HasIndex("OwnerId");

                    b.ToTable("transaction_log");
                });

            modelBuilder.Entity("aTES.Billing.DataAccess.DbUser", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<long>("Balance")
                        .HasColumnType("bigint")
                        .HasColumnName("amount");

                    b.Property<string>("PublicId")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("public-id");

                    b.Property<int>("Role")
                        .HasColumnType("integer")
                        .HasColumnName("role");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("username");

                    b.HasKey("Id");

                    b.ToTable("users");
                });

            modelBuilder.Entity("aTES.Billing.DataAccess.DbTask", b =>
                {
                    b.HasOne("aTES.Billing.DataAccess.DbUser", "AssignedUser")
                        .WithMany()
                        .HasForeignKey("AssignedUserId");

                    b.Navigation("AssignedUser");
                });

            modelBuilder.Entity("aTES.Billing.DataAccess.DbTransactionLogRecord", b =>
                {
                    b.HasOne("aTES.Billing.DataAccess.DbUser", "Owner")
                        .WithMany()
                        .HasForeignKey("OwnerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Owner");
                });
#pragma warning restore 612, 618
        }
    }
}
