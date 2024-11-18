using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace eCommerce.Orders.Infraestructure.Contexts.DbOrder
{
    public partial class DbOrderContext : DbContext
    {
        public DbOrderContext()
        {
        }

        public DbOrderContext(DbContextOptions<DbOrderContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Order> Orders { get; set; } = null!;
        public virtual DbSet<OrderDetail> OrderDetails { get; set; } = null!;
        public virtual DbSet<OrderPayment> OrderPayments { get; set; } = null!;
        public virtual DbSet<OrderShippingAddress> OrderShippingAddresses { get; set; } = null!;
        public virtual DbSet<OrderStatus> OrderStatuses { get; set; } = null!;
        public virtual DbSet<PaymentFranchise> PaymentFranchises { get; set; } = null!;
        public virtual DbSet<PaymentType> PaymentTypes { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=10.43.102.51;Database=EcommerceOrders;Trusted_Connection=False;User id=usr_order; password=Pru3b@s.123");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Order>(entity =>
            {
                entity.ToTable("Order");

                entity.Property(e => e.Id).HasMaxLength(100);

                entity.Property(e => e.Comment).HasMaxLength(150);

                entity.Property(e => e.CustomerEmail).HasMaxLength(50);

                entity.Property(e => e.CustomerId).HasMaxLength(50);

                entity.Property(e => e.Date).HasColumnType("datetime");

                entity.Property(e => e.PaymentRef).HasMaxLength(255);

                entity.Property(e => e.Total).HasColumnType("decimal(18, 0)");

                entity.HasOne(d => d.OrderStatus)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.OrderStatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Order_OrderStatus");
            });

            modelBuilder.Entity<OrderDetail>(entity =>
            {
                entity.ToTable("OrderDetail");

                entity.Property(e => e.OrderId).HasMaxLength(100);

                entity.Property(e => e.Price).HasColumnType("decimal(18, 0)");

                entity.Property(e => e.ProductDescription).HasMaxLength(250);

                entity.Property(e => e.ProductName).HasMaxLength(250);

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.OrderDetails)
                    .HasForeignKey(d => d.OrderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OrderDetail_Order");
            });

            modelBuilder.Entity<OrderPayment>(entity =>
            {
                entity.ToTable("OrderPayment");

                entity.Property(e => e.Amount).HasMaxLength(100);

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.CustIdCliente).HasMaxLength(100);

                entity.Property(e => e.CustomerIp).HasMaxLength(100);

                entity.Property(e => e.OrderId).HasMaxLength(100);

                entity.Property(e => e.PaymentFranchiseId).HasMaxLength(4);

                entity.Property(e => e.PaymentTypeId).HasMaxLength(4);

                entity.Property(e => e.RefPayco).HasMaxLength(100);

                entity.Property(e => e.ResponseCode).HasMaxLength(100);

                entity.Property(e => e.ResponseReason).HasMaxLength(100);

                entity.Property(e => e.TransactionDate).HasMaxLength(100);

                entity.Property(e => e.TransactionId).HasMaxLength(100);

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.OrderPayments)
                    .HasForeignKey(d => d.OrderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OrderPayment_Order");

                entity.HasOne(d => d.PaymentFranchise)
                    .WithMany(p => p.OrderPayments)
                    .HasForeignKey(d => d.PaymentFranchiseId)
                    .HasConstraintName("FK_PaymentFranchise_Order");

                entity.HasOne(d => d.PaymentType)
                    .WithMany(p => p.OrderPayments)
                    .HasForeignKey(d => d.PaymentTypeId)
                    .HasConstraintName("FK_PaymentType_Order");
            });

            modelBuilder.Entity<OrderShippingAddress>(entity =>
            {
                entity.ToTable("OrderShippingAddress");

                entity.Property(e => e.Address).HasMaxLength(250);

                entity.Property(e => e.AddressDesc).HasMaxLength(250);

                entity.Property(e => e.City).HasMaxLength(100);

                entity.Property(e => e.Deparment).HasMaxLength(100);

                entity.Property(e => e.Email).HasMaxLength(250);

                entity.Property(e => e.FirstName).HasMaxLength(100);

                entity.Property(e => e.LastName).HasMaxLength(100);

                entity.Property(e => e.OrderId).HasMaxLength(100);

                entity.Property(e => e.OtherInformation).HasMaxLength(250);

                entity.Property(e => e.Phone).HasMaxLength(20);

                entity.Property(e => e.PostalCode).HasMaxLength(20);

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.OrderShippingAddresses)
                    .HasForeignKey(d => d.OrderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OrderShippingAddress_Order");
            });

            modelBuilder.Entity<OrderStatus>(entity =>
            {
                entity.ToTable("OrderStatus");

                entity.HasIndex(e => e.Id, "UQ__OrderSta__3214EC06EED0FCA5")
                    .IsUnique();

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Descripction)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Status)
                    .HasMaxLength(20)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<PaymentFranchise>(entity =>
            {
                entity.ToTable("PaymentFranchise");

                entity.Property(e => e.Id).HasMaxLength(4);

                entity.Property(e => e.Description).HasMaxLength(50);
            });

            modelBuilder.Entity<PaymentType>(entity =>
            {
                entity.ToTable("PaymentType");

                entity.Property(e => e.Id).HasMaxLength(4);

                entity.Property(e => e.Description).HasMaxLength(50);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
