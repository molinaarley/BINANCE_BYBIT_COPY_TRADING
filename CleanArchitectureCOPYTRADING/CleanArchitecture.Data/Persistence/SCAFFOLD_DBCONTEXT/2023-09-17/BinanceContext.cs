using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace CleanArchitecture.Infrastructure.Persistence.SCAFFOLD_DBCONTEXT.2023-09-17
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

        public virtual DbSet<AspNetRole> AspNetRoles { get; set; } = null!;
        public virtual DbSet<AspNetRoleClaim> AspNetRoleClaims { get; set; } = null!;
        public virtual DbSet<AspNetUser> AspNetUsers { get; set; } = null!;
        public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; } = null!;
        public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; } = null!;
        public virtual DbSet<AspNetUserToken> AspNetUserTokens { get; set; } = null!;
        public virtual DbSet<BinanceByBitOrder> BinanceByBitOrders { get; set; } = null!;
        public virtual DbSet<BinanceByBitOrderAudit> BinanceByBitOrderAudits { get; set; } = null!;
        public virtual DbSet<BinanceByBitUser> BinanceByBitUsers { get; set; } = null!;
        public virtual DbSet<BinanceMonitoringProcess> BinanceMonitoringProcesses { get; set; } = null!;
        public virtual DbSet<BinanceOrder> BinanceOrders { get; set; } = null!;
        public virtual DbSet<BinanceOrderAudit> BinanceOrderAudits { get; set; } = null!;
        public virtual DbSet<BinanceTrader> BinanceTraders { get; set; } = null!;
        public virtual DbSet<BinanceTraderTypeDatum> BinanceTraderTypeData { get; set; } = null!;

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
            modelBuilder.Entity<AspNetRole>(entity =>
            {
                entity.HasIndex(e => e.NormalizedName, "RoleNameIndex")
                    .IsUnique()
                    .HasFilter("([NormalizedName] IS NOT NULL)");

                entity.Property(e => e.Name).HasMaxLength(256);

                entity.Property(e => e.NormalizedName).HasMaxLength(256);
            });

            modelBuilder.Entity<AspNetRoleClaim>(entity =>
            {
                entity.HasIndex(e => e.RoleId, "IX_AspNetRoleClaims_RoleId");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.AspNetRoleClaims)
                    .HasForeignKey(d => d.RoleId);
            });

            modelBuilder.Entity<AspNetUser>(entity =>
            {
                entity.HasIndex(e => e.NormalizedEmail, "EmailIndex");

                entity.HasIndex(e => e.NormalizedUserName, "UserNameIndex")
                    .IsUnique()
                    .HasFilter("([NormalizedUserName] IS NOT NULL)");

                entity.Property(e => e.Email).HasMaxLength(256);

                entity.Property(e => e.NormalizedEmail).HasMaxLength(256);

                entity.Property(e => e.NormalizedUserName).HasMaxLength(256);

                entity.Property(e => e.UserName).HasMaxLength(256);

                entity.HasMany(d => d.Roles)
                    .WithMany(p => p.Users)
                    .UsingEntity<Dictionary<string, object>>(
                        "AspNetUserRole",
                        l => l.HasOne<AspNetRole>().WithMany().HasForeignKey("RoleId"),
                        r => r.HasOne<AspNetUser>().WithMany().HasForeignKey("UserId"),
                        j =>
                        {
                            j.HasKey("UserId", "RoleId");

                            j.ToTable("AspNetUserRoles");

                            j.HasIndex(new[] { "RoleId" }, "IX_AspNetUserRoles_RoleId");
                        });
            });

            modelBuilder.Entity<AspNetUserClaim>(entity =>
            {
                entity.HasIndex(e => e.UserId, "IX_AspNetUserClaims_UserId");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserClaims)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<AspNetUserLogin>(entity =>
            {
                entity.HasKey(e => new { e.LoginProvider, e.ProviderKey });

                entity.HasIndex(e => e.UserId, "IX_AspNetUserLogins_UserId");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserLogins)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<AspNetUserToken>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.LoginProvider, e.Name });

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserTokens)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<BinanceByBitOrder>(entity =>
            {
                entity.ToTable("Binance_ByBit_Order");

                entity.HasIndex(e => e.BinanceOrderId, "IX_Binance_ByBit_Order_Binance_Order_Id");

                entity.HasIndex(e => e.IdTelegrame, "IX_Binance_ByBit_Order_Id_Telegrame");

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

            modelBuilder.Entity<BinanceByBitOrderAudit>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("Binance_ByBit_Order_Audit");

                entity.Property(e => e.BinanceOrderId).HasColumnName("Binance_Order_Id");

                entity.Property(e => e.ByBitOrderId)
                    .HasMaxLength(250)
                    .IsUnicode(false)
                    .HasColumnName("ByBit_OrderId");

                entity.Property(e => e.ByBitOrderLinkId)
                    .HasMaxLength(250)
                    .IsUnicode(false)
                    .HasColumnName("ByBit_OrderLinkId");

                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.Property(e => e.DeleteOn)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IdTelegrame).HasColumnName("Id_Telegrame");
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

                entity.Property(e => e.Isactive).HasDefaultValueSql("(CONVERT([bit],(0)))");

                entity.Property(e => e.LastName).HasMaxLength(250);

                entity.Property(e => e.Name).HasMaxLength(250);

                entity.Property(e => e.PhoneNumberTelegrame)
                    .HasMaxLength(250)
                    .HasColumnName("PhoneNumber_Telegrame");

                entity.Property(e => e.SecretKey)
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<BinanceMonitoringProcess>(entity =>
            {
                entity.ToTable("Binance_MonitoringProcess");

                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.Property(e => e.EndDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<BinanceOrder>(entity =>
            {
                entity.ToTable("Binance_Order");

                entity.HasIndex(e => e.EncryptedUid, "IX_Binance_Order_EncryptedUid");

                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.Property(e => e.EncryptedUid)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Side)
                    .HasMaxLength(10)
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

            modelBuilder.Entity<BinanceOrderAudit>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("Binance_Order_Audit");

                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.Property(e => e.DeleteOn)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.EncryptedUid)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Side)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.Symbol)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");
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

                entity.Property(e => e.UpdateTime)
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<BinanceTraderTypeDatum>(entity =>
            {
                entity.ToTable("Binance_Trader_TypeData");

                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.EncryptedUid)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.TypeData)
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.HasOne(d => d.EncryptedU)
                    .WithMany(p => p.BinanceTraderTypeData)
                    .HasForeignKey(d => d.EncryptedUid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Binance_Trader_TypeData_Binance_Trader");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
