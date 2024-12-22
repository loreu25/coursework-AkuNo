using Microsoft.EntityFrameworkCore;
using WpfApp1.Models;

namespace WpfApp1.Data
{
    public class StoreContext : DbContext
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<Sale> Sales { get; set; }
        public DbSet<Category> Categories { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=store.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Description).HasMaxLength(500);
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired();
                entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
                entity.Property(e => e.ImageData).HasColumnType("BLOB").IsRequired(false);
                entity.Property(e => e.ImagePath).HasMaxLength(500).IsRequired(false);

                entity.HasOne(e => e.Category)
                      .WithMany(c => c.Products)
                      .HasForeignKey(e => e.CategoryId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Sale>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.TotalPrice).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Date).IsRequired();
                
                entity.HasOne(e => e.Product)
                      .WithMany()
                      .HasForeignKey(e => e.ProductId)
                      .OnDelete(DeleteBehavior.Cascade); 
            });

            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Молочные продукты", Description = "Молоко, сыр, йогурты, творог" },
                new Category { Id = 2, Name = "Мясо и птица", Description = "Свежее мясо, птица, полуфабрикаты" },
                new Category { Id = 3, Name = "Хлеб и выпечка", Description = "Хлебобулочные изделия" },
                new Category { Id = 4, Name = "Фрукты и овощи", Description = "Свежие фрукты и овощи" },
                new Category { Id = 5, Name = "Напитки", Description = "Соки, воды, газированные напитки" },
                new Category { Id = 6, Name = "Бакалея", Description = "Крупы, макароны, масла, консервы" },
                new Category { Id = 7, Name = "Кондитерские изделия", Description = "Конфеты, печенье, шоколад" },
                new Category { Id = 8, Name = "Замороженные продукты", Description = "Замороженные овощи, полуфабрикаты" }
            );
        }
    }
}
