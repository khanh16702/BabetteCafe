using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Nhom7_N03_TrangWebQuanCafe.Models
{
    public partial class CafeWebsiteContext : DbContext
    {
        public CafeWebsiteContext()
        {
        }

        public CafeWebsiteContext(DbContextOptions<CafeWebsiteContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Account> Accounts { get; set; } = null!;
        public virtual DbSet<Banner> Banners { get; set; } = null!;
        public virtual DbSet<BookTime> BookTimes { get; set; } = null!;
        public virtual DbSet<Cart> Carts { get; set; } = null!;
        public virtual DbSet<ContactMail> ContactMails { get; set; } = null!;
        public virtual DbSet<Customer> Customers { get; set; } = null!;
        public virtual DbSet<DiscountCode> DiscountCodes { get; set; } = null!;
        public virtual DbSet<Gallery> Galleries { get; set; } = null!;
        public virtual DbSet<ImportReceipt> ImportReceipts { get; set; } = null!;
        public virtual DbSet<ImportReceiptDetail> ImportReceiptDetails { get; set; } = null!;
        public virtual DbSet<ImportedGood> ImportedGoods { get; set; } = null!;
        public virtual DbSet<News> News { get; set; } = null!;
        public virtual DbSet<NewsAndCategory> NewsAndCategories { get; set; } = null!;
        public virtual DbSet<NewsAndTag> NewsAndTags { get; set; } = null!;
        public virtual DbSet<NewsCategory> NewsCategories { get; set; } = null!;
        public virtual DbSet<NewsTag> NewsTags { get; set; } = null!;
        public virtual DbSet<Product> Products { get; set; } = null!;
        public virtual DbSet<ProductAndCategory> ProductAndCategories { get; set; } = null!;
        public virtual DbSet<ProductCategory> ProductCategories { get; set; } = null!;
        public virtual DbSet<ProviderSide> ProviderSides { get; set; } = null!;
        public virtual DbSet<Role> Roles { get; set; } = null!;
        public virtual DbSet<SaleReceiptAndDiscount> SaleReceiptAndDiscounts { get; set; } = null!;
        public virtual DbSet<SalesReceipt> SalesReceipts { get; set; } = null!;
        public virtual DbSet<TableShop> TableShops { get; set; } = null!;
        public virtual DbSet<TableShopAndAccount> TableShopAndAccounts { get; set; } = null!;
        public virtual DbSet<staff> staff { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Data Source=DESKTOP-JLDOEOK\\MAYTINH;Initial Catalog=CafeWebsite;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>(entity =>
            {
                entity.ToTable("Account");

                entity.Property(e => e.Address).HasMaxLength(255);

                entity.Property(e => e.DisplayName).HasMaxLength(255);

                entity.Property(e => e.Email)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.FirstName).HasMaxLength(50);

                entity.Property(e => e.Image).HasMaxLength(255);

                entity.Property(e => e.LastName).HasMaxLength(50);

                entity.Property(e => e.Password)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Phone)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Username)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.Accounts)
                    .HasForeignKey(d => d.RoleId)
                    .HasConstraintName("FK__Account__RoleId__04E4BC85");
            });

            modelBuilder.Entity<Banner>(entity =>
            {
                entity.ToTable("Banner");

                entity.Property(e => e.Content).HasMaxLength(255);

                entity.Property(e => e.CreatedBy).HasMaxLength(50);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Image).HasMaxLength(255);

                entity.Property(e => e.Title).HasMaxLength(50);

                entity.Property(e => e.UpdatedBy).HasMaxLength(50);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<BookTime>(entity =>
            {
                entity.ToTable("BookTime");

                entity.Property(e => e.BookTimeId).ValueGeneratedNever();
            });

            modelBuilder.Entity<Cart>(entity =>
            {
                entity.ToTable("Cart");

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.Carts)
                    .HasForeignKey(d => d.AccountId)
                    .HasConstraintName("FK__Cart__AccountId__5BE2A6F2");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.Carts)
                    .HasForeignKey(d => d.CustomerId)
                    .HasConstraintName("FK__Cart__CustomerId__5CD6CB2B");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.Carts)
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("FK__Cart__ProductId__5AEE82B9");

                entity.HasOne(d => d.SalesReceipt)
                    .WithMany(p => p.Carts)
                    .HasForeignKey(d => d.SalesReceiptId)
                    .HasConstraintName("FK__Cart__SalesRecei__5DCAEF64");
            });

            modelBuilder.Entity<ContactMail>(entity =>
            {
                entity.ToTable("ContactMail");

                entity.Property(e => e.Email)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.FirstName).HasMaxLength(255);

                entity.Property(e => e.LastName).HasMaxLength(255);

                entity.Property(e => e.Phone)
                    .HasMaxLength(20)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.ToTable("Customer");

                entity.Property(e => e.CreatedBy).HasMaxLength(50);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Name).HasMaxLength(255);

                entity.Property(e => e.Phone)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.UpdatedBy).HasMaxLength(50);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<DiscountCode>(entity =>
            {
                entity.ToTable("DiscountCode");

                entity.Property(e => e.ActiveDate).HasColumnType("datetime");

                entity.Property(e => e.CreatedBy).HasMaxLength(255);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.ExpireDate).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .HasMaxLength(20)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Gallery>(entity =>
            {
                entity.ToTable("Gallery");

                entity.Property(e => e.Format)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Path).HasMaxLength(255);

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.Galleries)
                    .HasForeignKey(d => d.AccountId)
                    .HasConstraintName("FK__Gallery__Account__47DBAE45");
            });

            modelBuilder.Entity<ImportReceipt>(entity =>
            {
                entity.ToTable("ImportReceipt");

                entity.Property(e => e.ImportDate).HasColumnType("datetime");

                entity.HasOne(d => d.Provider)
                    .WithMany(p => p.ImportReceipts)
                    .HasForeignKey(d => d.ProviderId)
                    .HasConstraintName("FK__ImportRec__Provi__6754599E");

                entity.HasOne(d => d.Staff)
                    .WithMany(p => p.ImportReceipts)
                    .HasForeignKey(d => d.StaffId)
                    .HasConstraintName("FK__ImportRec__Staff__66603565");
            });

            modelBuilder.Entity<ImportReceiptDetail>(entity =>
            {
                entity.ToTable("ImportReceiptDetail");

                entity.HasOne(d => d.ImportReceipt)
                    .WithMany(p => p.ImportReceiptDetails)
                    .HasForeignKey(d => d.ImportReceiptId)
                    .HasConstraintName("FK__ImportRec__Impor__6C190EBB");

                entity.HasOne(d => d.ImportedGood)
                    .WithMany(p => p.ImportReceiptDetails)
                    .HasForeignKey(d => d.ImportedGoodId)
                    .HasConstraintName("FK__ImportRec__Impor__6D0D32F4");
            });

            modelBuilder.Entity<ImportedGood>(entity =>
            {
                entity.ToTable("ImportedGood");

                entity.Property(e => e.Name).HasMaxLength(50);
            });

            modelBuilder.Entity<News>(entity =>
            {
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Image).HasMaxLength(200);

                entity.Property(e => e.Summary).HasMaxLength(255);

                entity.Property(e => e.Title).HasMaxLength(255);

                entity.Property(e => e.UpdatedBy).HasMaxLength(50);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.News)
                    .HasForeignKey(d => d.AccountId)
                    .HasConstraintName("FK__News__AccountId__14270015");
            });

            modelBuilder.Entity<NewsAndCategory>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("NewsAndCategory");

                entity.HasOne(d => d.NewsCategory)
                    .WithMany()
                    .HasForeignKey(d => d.NewsCategoryId)
                    .HasConstraintName("FK__NewsAndCa__NewsC__70DDC3D8");

                entity.HasOne(d => d.News)
                    .WithMany()
                    .HasForeignKey(d => d.NewsId)
                    .HasConstraintName("FK__NewsAndCa__NewsI__6FE99F9F");
            });

            modelBuilder.Entity<NewsAndTag>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("NewsAndTag");

                entity.HasOne(d => d.News)
                    .WithMany()
                    .HasForeignKey(d => d.NewsId)
                    .HasConstraintName("FK__NewsAndTa__NewsI__72C60C4A");

                entity.HasOne(d => d.NewsTag)
                    .WithMany()
                    .HasForeignKey(d => d.NewsTagId)
                    .HasConstraintName("FK__NewsAndTa__NewsT__73BA3083");
            });

            modelBuilder.Entity<NewsCategory>(entity =>
            {
                entity.ToTable("NewsCategory");

                entity.Property(e => e.CreatedBy).HasMaxLength(50);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Name).HasMaxLength(50);

                entity.Property(e => e.UpdatedBy).HasMaxLength(50);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<NewsTag>(entity =>
            {
                entity.ToTable("NewsTag");

                entity.Property(e => e.CreatedBy).HasMaxLength(50);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Name).HasMaxLength(20);

                entity.Property(e => e.UpdatedBy).HasMaxLength(50);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("Product");

                entity.Property(e => e.CreatedBy).HasMaxLength(50);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Image).HasMaxLength(255);

                entity.Property(e => e.Name).HasMaxLength(255);

                entity.Property(e => e.Summary).HasMaxLength(255);

                entity.Property(e => e.UpdatedBy).HasMaxLength(50);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<ProductAndCategory>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("ProductAndCategory");

                entity.HasOne(d => d.ProductCategory)
                    .WithMany()
                    .HasForeignKey(d => d.ProductCategoryId)
                    .HasConstraintName("FK__ProductAn__Produ__7C4F7684");

                entity.HasOne(d => d.Product)
                    .WithMany()
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("FK__ProductAn__Produ__7B5B524B");
            });

            modelBuilder.Entity<ProductCategory>(entity =>
            {
                entity.ToTable("ProductCategory");

                entity.Property(e => e.CreatedBy).HasMaxLength(50);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Name).HasMaxLength(255);

                entity.Property(e => e.UpdatedBy).HasMaxLength(50);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<ProviderSide>(entity =>
            {
                entity.HasKey(e => e.ProviderId)
                    .HasName("PK__Provider__B54C687DF98B17D0");

                entity.ToTable("ProviderSide");

                entity.Property(e => e.Name).HasMaxLength(255);
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("Role");

                entity.Property(e => e.Name).HasMaxLength(50);
            });

            modelBuilder.Entity<SaleReceiptAndDiscount>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("SaleReceiptAndDiscount");

                entity.HasOne(d => d.DiscountCode)
                    .WithMany()
                    .HasForeignKey(d => d.DiscountCodeId)
                    .HasConstraintName("FK__SaleRecei__Disco__797309D9");

                entity.HasOne(d => d.SalesReceipt)
                    .WithMany()
                    .HasForeignKey(d => d.SalesReceiptId)
                    .HasConstraintName("FK__SaleRecei__Sales__787EE5A0");
            });

            modelBuilder.Entity<SalesReceipt>(entity =>
            {
                entity.ToTable("SalesReceipt");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.SalesReceipts)
                    .HasForeignKey(d => d.AccountId)
                    .HasConstraintName("FK__SalesRece__Accou__208CD6FA");

                entity.HasOne(d => d.Staff)
                    .WithMany(p => p.SalesReceipts)
                    .HasForeignKey(d => d.StaffId)
                    .HasConstraintName("FK__SalesRece__Staff__5165187F");
            });

            modelBuilder.Entity<TableShop>(entity =>
            {
                entity.HasKey(e => e.TableId)
                    .HasName("PK__TableSho__7D5F01EEB8B2C6EF");

                entity.ToTable("TableShop");

                entity.Property(e => e.Status)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.HasOne(d => d.BookTime)
                    .WithMany(p => p.TableShops)
                    .HasForeignKey(d => d.BookTimeId)
                    .HasConstraintName("FK__TableShop__BookT__1F98B2C1");
            });

            modelBuilder.Entity<TableShopAndAccount>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("TableShopAndAccount");

                entity.HasOne(d => d.Account)
                    .WithMany()
                    .HasForeignKey(d => d.AccountId)
                    .HasConstraintName("FK__TableShop__Accou__76969D2E");

                entity.HasOne(d => d.Table)
                    .WithMany()
                    .HasForeignKey(d => d.TableId)
                    .HasConstraintName("FK__TableShop__Table__75A278F5");
            });

            modelBuilder.Entity<staff>(entity =>
            {
                entity.ToTable("Staff");

                entity.Property(e => e.Address).HasMaxLength(255);

                entity.Property(e => e.DateOfBirth).HasColumnType("datetime");

                entity.Property(e => e.FirstName).HasMaxLength(50);

                entity.Property(e => e.LastName).HasMaxLength(50);

                entity.Property(e => e.Phone)
                    .HasMaxLength(20)
                    .IsUnicode(false);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
