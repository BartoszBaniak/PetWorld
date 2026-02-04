using Microsoft.EntityFrameworkCore;
using PetWorld.Domain.Entities;

namespace PetWorld.Infrastructure.Data;

public class PetWorldDbContext : DbContext
{
    public PetWorldDbContext(DbContextOptions<PetWorldDbContext> options)
        : base(options)
    {
    }

    public DbSet<Product> Products => Set<Product>();
    public DbSet<ChatHistory> ChatHistories => Set<ChatHistory>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).IsRequired().HasMaxLength(1000);
            entity.Property(e => e.Category).IsRequired().HasMaxLength(100);
            entity.Property(e => e.PetType).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Brand).HasMaxLength(100);
            entity.Property(e => e.Price).HasPrecision(10, 2);
            entity.Property(e => e.ImageUrl).HasMaxLength(500);
            entity.Property(e => e.Tags).HasMaxLength(500);
        });

        modelBuilder.Entity<ChatHistory>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Pytanie).IsRequired().HasColumnType("TEXT");
            entity.Property(e => e.Odpowiedz).IsRequired().HasColumnType("TEXT");
            entity.Property(e => e.RecommendedProducts).HasColumnType("TEXT");
        });
    }
}
