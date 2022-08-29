﻿using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace RentCollection.NetAPI.Models
{
    public partial class RentCollectionContext : DbContext
    {
        public RentCollectionContext()
        {
        }

        public RentCollectionContext(DbContextOptions<RentCollectionContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Allocation> Allocations { get; set; }
        public virtual DbSet<AutomatedRaisedPayment> AutomatedRaisedPayments { get; set; }
        public virtual DbSet<Document> Documents { get; set; }
        public virtual DbSet<DocumentType> DocumentTypes { get; set; }
        public virtual DbSet<ElectricityMeterReading> ElectricityMeterReadings { get; set; }
        public virtual DbSet<Invoice> Invoices { get; set; }
        public virtual DbSet<InvoiceItem> InvoiceItems { get; set; }
        public virtual DbSet<InvoiceItemCategory> InvoiceItemCategories { get; set; }
        public virtual DbSet<ModeOfPayment> ModeOfPayments { get; set; }
        public virtual DbSet<Payment> Payments { get; set; }
        public virtual DbSet<Rental> Rentals { get; set; }
        public virtual DbSet<Tenant> Tenants { get; set; }
        public virtual DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("server=.\\sqlexpress;database=RentCollection;trusted_connection=true");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<Allocation>(entity =>
            {
                entity.ToTable("Allocation");

                entity.Property(e => e.AllocatedOn).HasColumnType("date");

                entity.HasOne(d => d.Rental)
                    .WithMany(p => p.Allocations)
                    .HasForeignKey(d => d.RentalId)
                    .HasConstraintName("FK__Allocatio__Renta__5EBF139D");
            });

            modelBuilder.Entity<AutomatedRaisedPayment>(entity =>
            {
                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.HasOne(d => d.Allocation)
                    .WithMany(p => p.AutomatedRaisedPayments)
                    .HasForeignKey(d => d.AllocationId)
                    .HasConstraintName("FK__Automated__Alloc__6E01572D");
            });

            modelBuilder.Entity<Document>(entity =>
            {
                entity.Property(e => e.DocumentName)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.HasOne(d => d.Tenant)
                    .WithMany(p => p.Documents)
                    .HasForeignKey(d => d.TenantId)
                    .HasConstraintName("FK__Documents__Tenan__59FA5E80");
            });

            modelBuilder.Entity<DocumentType>(entity =>
            {
                entity.ToTable("DocumentType");

                entity.HasIndex(e => new { e.UserId, e.Code }, "Document_Type")
                    .IsUnique();

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.DocumentTypes)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK__DocumentT__UserI__571DF1D5");
            });

            modelBuilder.Entity<ElectricityMeterReading>(entity =>
            {
                entity.HasKey(e => e.MeterReadingId)
                    .HasName("PK__Electric__AFB4FD99D1EC2469");

                entity.ToTable("ElectricityMeterReading");

                entity.Property(e => e.TakenOn).HasColumnType("date");

                entity.HasOne(d => d.Rental)
                    .WithMany(p => p.ElectricityMeterReadings)
                    .HasForeignKey(d => d.RentalId)
                    .HasConstraintName("FK__Electrici__Renta__619B8048");
            });

            modelBuilder.Entity<Invoice>(entity =>
            {
                entity.Property(e => e.InvoiceDate).HasColumnType("date");

                entity.HasOne(d => d.Allocation)
                    .WithMany(p => p.Invoices)
                    .HasForeignKey(d => d.AllocationId)
                    .HasConstraintName("FK__Invoices__Alloca__6477ECF3");
            });

            modelBuilder.Entity<InvoiceItem>(entity =>
            {
                entity.ToTable("InvoiceItem");

                entity.Property(e => e.Date).HasColumnType("date");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.HasOne(d => d.Invoice)
                    .WithMany(p => p.InvoiceItems)
                    .HasForeignKey(d => d.InvoiceId)
                    .HasConstraintName("FK__InvoiceIt__Invoi__6B24EA82");
            });

            modelBuilder.Entity<InvoiceItemCategory>(entity =>
            {
                entity.ToTable("InvoiceItemCategory");

                entity.HasIndex(e => new { e.UserId, e.Code }, "User_Invoice_Item_Category")
                    .IsUnique();

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.InvoiceItemCategories)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK__InvoiceIt__UserI__68487DD7");
            });

            modelBuilder.Entity<ModeOfPayment>(entity =>
            {
                entity.ToTable("ModeOfPayment");

                entity.Property(e => e.Code)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Payment>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.Description)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.PaymentId).ValueGeneratedOnAdd();

                entity.HasOne(d => d.Invoice)
                    .WithMany()
                    .HasForeignKey(d => d.InvoiceId)
                    .HasConstraintName("FK__Payments__Invoic__71D1E811");

                entity.HasOne(d => d.ModeOfPayment)
                    .WithMany()
                    .HasForeignKey(d => d.ModeOfPaymentId)
                    .HasConstraintName("FK__Payments__ModeOf__72C60C4A");
            });

            modelBuilder.Entity<Rental>(entity =>
            {
                entity.HasIndex(e => new { e.UserId, e.Title }, "Unique_Rental")
                    .IsUnique();

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Rentals)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK__Rentals__UserId__4F7CD00D");
            });

            modelBuilder.Entity<Tenant>(entity =>
            {
                entity.HasIndex(e => new { e.UserId, e.Contact }, "Unique_Contact")
                    .IsUnique();

                entity.Property(e => e.Contact)
                    .IsRequired()
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.Email)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.FullName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Tenants)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK__Tenants__UserId__534D60F1");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(e => e.Username, "UQ__Users__536C85E430BC9945")
                    .IsUnique();

                entity.HasIndex(e => e.Contact, "UQ__Users__F7C046656DD36D42")
                    .IsUnique();

                entity.Property(e => e.Contact)
                    .IsRequired()
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.FullName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Role)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
