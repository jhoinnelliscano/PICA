using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace eCommerce.Customers.Infraestructure.Contexts.DbCustomers
{
    public partial class DbCustomerContext : DbContext
    {
        public DbCustomerContext()
        {
        }

        public DbCustomerContext(DbContextOptions<DbCustomerContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Customer> Customers { get; set; } = null!;
        public virtual DbSet<CustomerAddress> CustomerAddresses { get; set; } = null!;
        public virtual DbSet<CustomerIdentType> CustomerIdentTypes { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=10.43.102.51;Database=EcommerceCustomers;Trusted_Connection=false;User id=usr_customers; password=Pru3b@s.123");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.ToTable("Customer");

                entity.Property(e => e.Id).HasMaxLength(50);

                entity.Property(e => e.AutenticationType).HasMaxLength(50);

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.Email).HasMaxLength(250);

                entity.Property(e => e.FirstName).HasMaxLength(50);

                entity.Property(e => e.IdentTypeId).HasMaxLength(3);

                entity.Property(e => e.Identification).HasMaxLength(20);

                entity.Property(e => e.LastName).HasMaxLength(50);

                entity.Property(e => e.Phone1).HasMaxLength(20);

                entity.Property(e => e.Phone2).HasMaxLength(20);

                entity.Property(e => e.SecondLastName).HasMaxLength(50);

                entity.Property(e => e.SecondName).HasMaxLength(50);

                entity.Property(e => e.Status).HasMaxLength(1);

                entity.Property(e => e.UserName).HasMaxLength(250);

                entity.HasOne(d => d.IdentType)
                    .WithMany(p => p.Customers)
                    .HasForeignKey(d => d.IdentTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_IdentificationType_Client");
            });

            modelBuilder.Entity<CustomerAddress>(entity =>
            {
                entity.ToTable("CustomerAddress");

                entity.Property(e => e.Address).HasMaxLength(250);

                entity.Property(e => e.AddressDesc).HasMaxLength(250);

                entity.Property(e => e.City).HasMaxLength(100);

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.CustomertId).HasMaxLength(50);

                entity.Property(e => e.Deparment).HasMaxLength(100);

                entity.Property(e => e.Email).HasMaxLength(250);

                entity.Property(e => e.FirstName).HasMaxLength(100);

                entity.Property(e => e.LastName).HasMaxLength(100);

                entity.Property(e => e.OtherInformation).HasMaxLength(250);

                entity.Property(e => e.Phone).HasMaxLength(20);

                entity.Property(e => e.PostalCode).HasMaxLength(20);

                entity.HasOne(d => d.Customert)
                    .WithMany(p => p.CustomerAddresses)
                    .HasForeignKey(d => d.CustomertId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ClientAdsress_Client");
            });

            modelBuilder.Entity<CustomerIdentType>(entity =>
            {
                entity.ToTable("CustomerIdentType");

                entity.Property(e => e.Id).HasMaxLength(3);

                entity.Property(e => e.Name).HasMaxLength(50);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
