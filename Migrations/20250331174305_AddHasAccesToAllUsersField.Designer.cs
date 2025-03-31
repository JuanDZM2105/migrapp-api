﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using migrapp_api.Data;

#nullable disable

namespace migrapp_api.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20250331174305_AddHasAccesToAllUsersField")]
    partial class AddHasAccesToAllUsersField
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("migrapp_api.Entidades.AssignedUser", b =>
                {
                    b.Property<int>("AssignedUserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("AssignedUserId"));

                    b.Property<DateTime>("AssignedAt")
                        .HasColumnType("datetime2");

                    b.Property<int>("ClientUserId")
                        .HasColumnType("int");

                    b.Property<string>("ProfessionalRole")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<int>("ProfessionalUserId")
                        .HasColumnType("int");

                    b.HasKey("AssignedUserId");

                    b.HasIndex("ClientUserId");

                    b.HasIndex("ProfessionalUserId");

                    b.ToTable("AssignedUsers");
                });

            modelBuilder.Entity("migrapp_api.Entidades.ColumnVisibility", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<string>("VisibleColumns")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("ColumnVisibilities");
                });

            modelBuilder.Entity("migrapp_api.Entidades.Document", b =>
                {
                    b.Property<int>("DocumentId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("DocumentId"));

                    b.Property<string>("DocumentType")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("FilePath")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("UploadedAt")
                        .HasColumnType("datetime2");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("DocumentId");

                    b.HasIndex("UserId");

                    b.ToTable("Documents");
                });

            modelBuilder.Entity("migrapp_api.Entidades.LegalProcess", b =>
                {
                    b.Property<int>("LegalProcessId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("LegalProcessId"));

                    b.Property<int>("ClientUserId")
                        .HasColumnType("int");

                    b.Property<decimal>("Cost")
                        .HasColumnType("decimal(18,2)");

                    b.Property<DateTime?>("EndDate")
                        .HasColumnType("datetime2");

                    b.Property<int?>("LawyerUserId")
                        .HasColumnType("int");

                    b.Property<string>("PaymentStatus")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<string>("ProcessStatus")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<string>("ProcessType")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("datetime2");

                    b.HasKey("LegalProcessId");

                    b.HasIndex("ClientUserId");

                    b.HasIndex("LawyerUserId");

                    b.ToTable("LegalProcesses");
                });

            modelBuilder.Entity("migrapp_api.Entidades.LegalProcessDocument", b =>
                {
                    b.Property<int>("LegalProcessDocumentId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("LegalProcessDocumentId"));

                    b.Property<int?>("DocumentId")
                        .HasColumnType("int");

                    b.Property<bool>("IsUploaded")
                        .HasColumnType("bit");

                    b.Property<int>("LegalProcessId")
                        .HasColumnType("int");

                    b.Property<string>("RequiredDocumentType")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("LegalProcessDocumentId");

                    b.HasIndex("DocumentId");

                    b.HasIndex("LegalProcessId");

                    b.ToTable("LegalProcessDocuments");
                });

            modelBuilder.Entity("migrapp_api.Entidades.User", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("UserId"));

                    b.Property<DateTime>("AccountCreated")
                        .HasColumnType("datetime2");

                    b.Property<string>("AccountStatus")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<DateTime?>("BirthDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Country")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(150)
                        .HasColumnType("nvarchar(150)");

                    b.Property<bool>("HasAccessToAllUsers")
                        .HasColumnType("bit");

                    b.Property<bool>("IsActiveNow")
                        .HasColumnType("bit");

                    b.Property<DateTime>("LastLogin")
                        .HasColumnType("datetime2");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Phone")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<string>("PhonePrefix")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserType")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.HasKey("UserId");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.ToTable("Users");
                });

            modelBuilder.Entity("migrapp_api.Entidades.UserLog", b =>
                {
                    b.Property<int>("UserLogId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("UserLogId"));

                    b.Property<DateTime>("ActionDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("ActionType")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<string>("IpAddress")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("UserLogId");

                    b.HasIndex("UserId");

                    b.ToTable("UserLogs");
                });

            modelBuilder.Entity("migrapp_api.Entidades.UserMfaCode", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("Expiration")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("UserMfaCodes");
                });

            modelBuilder.Entity("migrapp_api.Entidades.AssignedUser", b =>
                {
                    b.HasOne("migrapp_api.Entidades.User", "ClientUser")
                        .WithMany("AssignedProfessionals")
                        .HasForeignKey("ClientUserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("migrapp_api.Entidades.User", "ProfessionalUser")
                        .WithMany("AssignedClients")
                        .HasForeignKey("ProfessionalUserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("ClientUser");

                    b.Navigation("ProfessionalUser");
                });

            modelBuilder.Entity("migrapp_api.Entidades.ColumnVisibility", b =>
                {
                    b.HasOne("migrapp_api.Entidades.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("migrapp_api.Entidades.Document", b =>
                {
                    b.HasOne("migrapp_api.Entidades.User", "User")
                        .WithMany("Documents")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("migrapp_api.Entidades.LegalProcess", b =>
                {
                    b.HasOne("migrapp_api.Entidades.User", "ClientUser")
                        .WithMany("ClientLegalProcesses")
                        .HasForeignKey("ClientUserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("migrapp_api.Entidades.User", "LawyerUser")
                        .WithMany("LawyerLegalProcesses")
                        .HasForeignKey("LawyerUserId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.Navigation("ClientUser");

                    b.Navigation("LawyerUser");
                });

            modelBuilder.Entity("migrapp_api.Entidades.LegalProcessDocument", b =>
                {
                    b.HasOne("migrapp_api.Entidades.Document", "Document")
                        .WithMany()
                        .HasForeignKey("DocumentId");

                    b.HasOne("migrapp_api.Entidades.LegalProcess", "LegalProcess")
                        .WithMany("RequiredDocuments")
                        .HasForeignKey("LegalProcessId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Document");

                    b.Navigation("LegalProcess");
                });

            modelBuilder.Entity("migrapp_api.Entidades.UserLog", b =>
                {
                    b.HasOne("migrapp_api.Entidades.User", "User")
                        .WithMany("UserLogs")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("migrapp_api.Entidades.LegalProcess", b =>
                {
                    b.Navigation("RequiredDocuments");
                });

            modelBuilder.Entity("migrapp_api.Entidades.User", b =>
                {
                    b.Navigation("AssignedClients");

                    b.Navigation("AssignedProfessionals");

                    b.Navigation("ClientLegalProcesses");

                    b.Navigation("Documents");

                    b.Navigation("LawyerLegalProcesses");

                    b.Navigation("UserLogs");
                });
#pragma warning restore 612, 618
        }
    }
}
