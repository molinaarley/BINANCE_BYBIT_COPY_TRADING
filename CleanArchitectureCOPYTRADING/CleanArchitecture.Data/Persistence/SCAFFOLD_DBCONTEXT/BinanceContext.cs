using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace CleanArchitecture.WEB
{
    public partial class BinanceContext : DbContext
    {
        public BinanceContext()
        {
        }

        public BinanceContext(DbContextOptions<BinanceContext> options)
            : base(options)
        {
        }

        public virtual DbSet<BinanceByBitOrder> BinanceByBitOrders { get; set; } = null!;
        public virtual DbSet<BinanceByBitUser> BinanceByBitUsers { get; set; } = null!;
        public virtual DbSet<BinanceOrder> BinanceOrders { get; set; } = null!;
        public virtual DbSet<BinanceTrader> BinanceTraders { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Password=xiomaraA1;Persist Security Info=True;User ID=linagma32046com26487_artelcom;Initial Catalog=Binance;Data Source=DESKTOP-H61MD4F;TrustServerCertificate=true");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BinanceByBitOrder>(entity =>
            {
                entity.ToTable("Binance_ByBit_Order");

                entity.Property(e => e.BinanceOrderId).HasColumnName("Binance_Order_Id");

                entity.Property(e => e.ByBitOrderId)
                    .HasMaxLength(250)
                    .IsUnicode(false)
                    .HasColumnName("ByBit_OrderId");

                entity.Property(e => e.ByBitOrderLinkId)
                    .HasMaxLength(250)
                    .IsUnicode(false)
                    .HasColumnName("ByBit_OrderLinkId");

                entity.Property(e => e.CreatedOn)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IdTelegrame).HasColumnName("Id_Telegrame");

                entity.HasOne(d => d.BinanceOrder)
                    .WithMany(p => p.BinanceByBitOrders)
                    .HasForeignKey(d => d.BinanceOrderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Binance_ByBit_Order_Binance_Order");

                entity.HasOne(d => d.IdTelegrameNavigation)
                    .WithMany(p => p.BinanceByBitOrders)
                    .HasForeignKey(d => d.IdTelegrame)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Binance_ByBit_Order_Binance_ByBit_Users");
            });

            modelBuilder.Entity<BinanceByBitUser>(entity =>
            {
                entity.HasKey(e => e.IdTelegrame);

                entity.ToTable("Binance_ByBit_Users");

                entity.Property(e => e.IdTelegrame)
                    .ValueGeneratedNever()
                    .HasColumnName("Id_Telegrame");

                entity.Property(e => e.ApiKey)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Createdate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.EmailBinance)
                    .HasMaxLength(250)
                    .HasColumnName("Email_Binance");

                entity.Property(e => e.EmailTelegrame)
                    .HasMaxLength(250)
                    .HasColumnName("Email_Telegrame");

                entity.Property(e => e.Isactive).HasDefaultValueSql("(CONVERT([bit],(0)))");

                entity.Property(e => e.LastName).HasMaxLength(250);

                entity.Property(e => e.Name).HasMaxLength(250);

                entity.Property(e => e.PhoneNumberTelegrame)
                    .HasMaxLength(250)
                    .HasColumnName("PhoneNumber_Telegrame");

                entity.Property(e => e.SecretKey)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.UserNameTelegrame)
                    .HasMaxLength(250)
                    .HasColumnName("UserName_Telegrame");
            });

            modelBuilder.Entity<BinanceOrder>(entity =>
            {
                entity.ToTable("Binance_Order");

                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.Property(e => e.EncryptedUid)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Symbol)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");

                entity.HasOne(d => d.EncryptedU)
                    .WithMany(p => p.BinanceOrders)
                    .HasForeignKey(d => d.EncryptedUid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Binance_Order_Binance_Trader");
            });

            modelBuilder.Entity<BinanceTrader>(entity =>
            {
                entity.HasKey(e => e.EncryptedUid);

                entity.ToTable("Binance_Trader");

                entity.Property(e => e.EncryptedUid)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.NickName)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("nickName");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
