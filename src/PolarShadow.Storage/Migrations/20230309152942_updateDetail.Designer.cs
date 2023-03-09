﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PolarShadow.Storage;

#nullable disable

namespace PolarShadow.Storage.Migrations
{
    [DbContext(typeof(PolarShadowDbContext))]
    [Migration("20230309152942_updateDetail")]
    partial class updateDetail
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "7.0.1");

            modelBuilder.Entity("PolarShadow.Core.WatchRecord", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("EpisodeName")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<long?>("Position")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("Name");

                    b.ToTable("Record");
                });

            modelBuilder.Entity("PolarShadow.Storage.VideoDetailEntity", b =>
                {
                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .HasColumnType("TEXT");

                    b.Property<string>("DetailSrc")
                        .HasColumnType("TEXT");

                    b.Property<string>("ImageSrc")
                        .HasColumnType("TEXT");

                    b.Property<string>("Seasons")
                        .HasColumnType("TEXT");

                    b.Property<string>("SiteName")
                        .HasColumnType("TEXT");

                    b.HasKey("Name");

                    b.ToTable("MyCollection");
                });
#pragma warning restore 612, 618
        }
    }
}
