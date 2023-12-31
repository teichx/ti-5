﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TI5yncronizer.Server.Context;

#nullable disable

namespace TI5yncronizer.Server.Migrations
{
    [DbContext(typeof(DataContext))]
    [Migration("20231123004230_AddUniqueOnListenerModel")]
    partial class AddUniqueOnListenerModel
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "7.0.13");

            modelBuilder.Entity("TI5yncronizer.Server.Model.ListenerModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasColumnName("id");

                    b.Property<string>("DeviceIdentifier")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("TEXT")
                        .HasColumnName("device_identifier");

                    b.Property<string>("LocalPath")
                        .IsRequired()
                        .HasMaxLength(512)
                        .HasColumnType("TEXT")
                        .HasColumnName("local_path");

                    b.Property<string>("ServerPath")
                        .IsRequired()
                        .HasMaxLength(512)
                        .HasColumnType("TEXT")
                        .HasColumnName("server_path");

                    b.HasKey("Id")
                        .HasName("pk_listener");

                    b.HasIndex("DeviceIdentifier", "LocalPath", "ServerPath")
                        .IsUnique()
                        .HasDatabaseName("ix_listener_device_identifier_local_path_server_path");

                    b.ToTable("listener", (string)null);
                });

            modelBuilder.Entity("TI5yncronizer.Server.Model.PendingSynchronizeModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasColumnName("id");

                    b.Property<int>("Action")
                        .HasColumnType("INTEGER")
                        .HasColumnName("action");

                    b.Property<string>("DeviceIdentifier")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("TEXT")
                        .HasColumnName("device_identifier");

                    b.Property<string>("LocalPath")
                        .IsRequired()
                        .HasMaxLength(512)
                        .HasColumnType("TEXT")
                        .HasColumnName("local_path");

                    b.Property<string>("OldLocalPath")
                        .HasMaxLength(512)
                        .HasColumnType("TEXT")
                        .HasColumnName("old_local_path");

                    b.HasKey("Id")
                        .HasName("pk_pending_synchronizer");

                    b.ToTable("pending_synchronizer", (string)null);
                });
#pragma warning restore 612, 618
        }
    }
}
