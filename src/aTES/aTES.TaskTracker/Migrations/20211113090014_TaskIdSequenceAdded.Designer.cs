﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using aTES.TaskTracker.DataLayer;

namespace aTES.TaskTracker.Migrations
{
    [DbContext(typeof(TasksDbContext))]
    [Migration("20211113090014_TaskIdSequenceAdded")]
    partial class TaskIdSequenceAdded
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.10")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            modelBuilder.HasSequence<int>("tasks-jira-id");

            modelBuilder.Entity("aTES.TaskTracker.DataLayer.DbTask", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<Guid>("AssignedUserId")
                        .HasColumnType("uuid")
                        .HasColumnName("assigned_user_id");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("description");

                    b.Property<bool>("IsCompeleted")
                        .HasColumnType("boolean")
                        .HasColumnName("is_completed");

                    b.Property<int>("SequenceValue")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("sequence_value")
                        .HasDefaultValueSql("nextval('\"tasks-jira-id\"')");

                    b.HasKey("Id");

                    b.HasIndex("AssignedUserId");

                    b.ToTable("tasks");
                });

            modelBuilder.Entity("aTES.TaskTracker.DataLayer.DbUser", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<string>("PublicId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("Role")
                        .HasColumnType("integer");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("users");
                });

            modelBuilder.Entity("aTES.TaskTracker.DataLayer.DbTask", b =>
                {
                    b.HasOne("aTES.TaskTracker.DataLayer.DbUser", "AssignedUser")
                        .WithMany()
                        .HasForeignKey("AssignedUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("AssignedUser");
                });
#pragma warning restore 612, 618
        }
    }
}