﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Monitor.Data;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Monitor.Data.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20231206191244_AddLastCheckedToReport")]
    partial class AddLastCheckedToReport
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.9")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Monitor.Data.Entities.Config", b =>
                {
                    b.Property<string>("Key")
                        .HasColumnType("text");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("Type")
                        .HasColumnType("integer");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Key");

                    b.ToTable("Config");
                });

            modelBuilder.Entity("Monitor.Data.Entities.Instance", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Instances");
                });

            modelBuilder.Entity("Monitor.Data.Entities.Report", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTimeOffset>("DateTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTimeOffset>("LastChecked")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("ReportTypeId")
                        .HasColumnType("integer");

                    b.Property<int>("StatusId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("ReportTypeId");

                    b.HasIndex("StatusId");

                    b.ToTable("Reports");
                });

            modelBuilder.Entity("Monitor.Data.Entities.ReportStatus", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("ReportStatus");
                });

            modelBuilder.Entity("Monitor.Data.Entities.ReportType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("ReportTypes");
                });

            modelBuilder.Entity("Monitor.Data.Entities.Site", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("InstanceId")
                        .HasColumnType("integer");

                    b.Property<string>("ParentSiteId")
                        .HasColumnType("text");

                    b.Property<int>("ReportId")
                        .HasColumnType("integer");

                    b.Property<string>("SiteId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("SiteName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("InstanceId");

                    b.HasIndex("ReportId");

                    b.ToTable("Sites");
                });

            modelBuilder.Entity("Monitor.Data.Entities.Study", b =>
                {
                    b.Property<int>("RedCapId")
                        .HasColumnType("integer");

                    b.Property<string>("ApiKey")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("InstanceId")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("StudyCapacityAlert")
                        .HasColumnType("boolean");

                    b.Property<bool>("StudyCapacityAlertsActivated")
                        .HasColumnType("boolean");

                    b.Property<TimeSpan>("StudyCapacityJobFrequency")
                        .HasColumnType("interval");

                    b.Property<DateTimeOffset>("StudyCapacityLastChecked")
                        .HasColumnType("timestamp with time zone");

                    b.Property<double>("StudyCapacityThreshold")
                        .HasColumnType("double precision");

                    b.HasKey("RedCapId");

                    b.HasIndex("InstanceId");

                    b.ToTable("Studies");
                });

            modelBuilder.Entity("Monitor.Data.Entities.StudyGroup", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("PlannedSize")
                        .HasColumnType("integer");

                    b.Property<int>("StudyRedCapId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("StudyRedCapId");

                    b.ToTable("StudyGroups");
                });

            modelBuilder.Entity("Monitor.Data.Entities.StudyUser", b =>
                {
                    b.Property<int>("StudyId")
                        .HasColumnType("integer");

                    b.Property<string>("UserId")
                        .HasColumnType("text");

                    b.HasKey("StudyId", "UserId");

                    b.ToTable("StudyUsers");
                });

            modelBuilder.Entity("Monitor.Data.Entities.Report", b =>
                {
                    b.HasOne("Monitor.Data.Entities.ReportType", "ReportType")
                        .WithMany()
                        .HasForeignKey("ReportTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Monitor.Data.Entities.ReportStatus", "Status")
                        .WithMany()
                        .HasForeignKey("StatusId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ReportType");

                    b.Navigation("Status");
                });

            modelBuilder.Entity("Monitor.Data.Entities.Site", b =>
                {
                    b.HasOne("Monitor.Data.Entities.Instance", "Instance")
                        .WithMany()
                        .HasForeignKey("InstanceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Monitor.Data.Entities.Report", null)
                        .WithMany("Sites")
                        .HasForeignKey("ReportId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Instance");
                });

            modelBuilder.Entity("Monitor.Data.Entities.Study", b =>
                {
                    b.HasOne("Monitor.Data.Entities.Instance", "Instance")
                        .WithMany()
                        .HasForeignKey("InstanceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Instance");
                });

            modelBuilder.Entity("Monitor.Data.Entities.StudyGroup", b =>
                {
                    b.HasOne("Monitor.Data.Entities.Study", "Study")
                        .WithMany("StudyGroups")
                        .HasForeignKey("StudyRedCapId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Study");
                });

            modelBuilder.Entity("Monitor.Data.Entities.StudyUser", b =>
                {
                    b.HasOne("Monitor.Data.Entities.Study", "Study")
                        .WithMany("Users")
                        .HasForeignKey("StudyId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Study");
                });

            modelBuilder.Entity("Monitor.Data.Entities.Report", b =>
                {
                    b.Navigation("Sites");
                });

            modelBuilder.Entity("Monitor.Data.Entities.Study", b =>
                {
                    b.Navigation("StudyGroups");

                    b.Navigation("Users");
                });
#pragma warning restore 612, 618
        }
    }
}
