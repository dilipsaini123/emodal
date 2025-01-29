﻿// <auto-generated />
using System;
using ContainerLockAPI.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace ContainerLockAPI.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20250121112951_RemoveEx")]
    partial class RemoveEx
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("ContainerLockAPI.Models.ContainerLock", b =>
                {
                    b.Property<string>("ContainerId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("LockTimestamp")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("GETUTCDATE()");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ContainerId");

                    b.ToTable("ContainerLock");
                });
#pragma warning restore 612, 618
        }
    }
}
