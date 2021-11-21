﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using aTES.Analytics.DataAccess;

namespace aTES.Analytics.Migrations
{
    [DbContext(typeof(AnalyticsDbContext))]
    [Migration("20211121133200_Tasks_attributes_updated")]
    partial class Tasks_attributes_updated
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.10")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            modelBuilder.Entity("aTES.Analytics.DataAccess.DbTask", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<DateTime>("ClosedAt")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("closed_at");

                    b.Property<long>("Cost")
                        .HasColumnType("bigint")
                        .HasColumnName("cost");

                    b.Property<string>("PublicId")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("public_id");

                    b.HasKey("Id");

                    b.ToTable("Tasks");
                });

            modelBuilder.Entity("aTES.Analytics.DataAccess.DbUser", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<long>("Balance")
                        .HasColumnType("bigint")
                        .HasColumnName("balance");

                    b.Property<string>("PublicId")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("public_id");

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
#pragma warning restore 612, 618
        }
    }
}
