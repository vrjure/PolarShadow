﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PolarShadow.Storage;

#nullable disable

namespace PolarShadow.Storage.Sqlite.Migrations.Migrations
{
    [DbContext(typeof(PolarShadowDbContext))]
    partial class PolarShadowDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.7");

            modelBuilder.Entity("PolarShadow.Services.HistoryModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<long>("Progress")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ProgressDesc")
                        .HasColumnType("TEXT");

                    b.Property<int>("ProgressIndex")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ResourceName")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("UpdateTime")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("ResourceName");

                    b.ToTable("Histories");
                });

            modelBuilder.Entity("PolarShadow.Services.ResourceModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Description")
                        .HasColumnType("TEXT");

                    b.Property<string>("FromRequest")
                        .HasColumnType("TEXT");

                    b.Property<string>("ImageSrc")
                        .HasColumnType("TEXT");

                    b.Property<string>("ImageSrcHeaders")
                        .HasColumnType("TEXT");

                    b.Property<int>("Level")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<int>("ParentId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Request")
                        .HasColumnType("TEXT");

                    b.Property<int>("RootId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Site")
                        .HasColumnType("TEXT");

                    b.Property<string>("Src")
                        .HasColumnType("TEXT");

                    b.Property<string>("SrcType")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Resources");
                });

            modelBuilder.Entity("PolarShadow.Storage.PreferenceEntity", b =>
                {
                    b.Property<string>("Key")
                        .HasColumnType("TEXT");

                    b.Property<string>("Value")
                        .HasColumnType("TEXT");

                    b.HasKey("Key");

                    b.ToTable("Preferences");
                });
#pragma warning restore 612, 618
        }
    }
}
