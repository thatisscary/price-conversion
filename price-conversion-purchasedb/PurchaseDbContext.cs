namespace price_conversion_purchasedb
{
    using Microsoft.EntityFrameworkCore;
    using price_conversion_purchasedb.Entities;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using System.Runtime.CompilerServices;
    using Microsoft.Extensions.Logging;

    public class PurchaseDbContext : DbContext
    {
        public PurchaseDbContext(DbContextOptions<PurchaseDbContext> options) : base(options)
        {
        }

        public DbSet<Purchase> Purchases => Set<Purchase>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            DefinePurchaseEntity(modelBuilder.Entity<Purchase>());
        }

        

        public static void DefinePurchaseEntity(EntityTypeBuilder<Purchase> entity)
        {
            entity.ToTable("Purchase")
                .HasKey(e => e.PurchaseId);
            entity.Property(e => e.Description)
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(e => e.TotalAmount)
                .IsRequired()
                .HasColumnType("decimal(10,2)");
            entity.Property(e => e.TransactionDate)
                .HasColumnType("datetime2")
                .IsRequired();
        }
    }
}