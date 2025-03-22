using KoaLaDessertWeb.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace KoaLaDessertWeb.Tools.DBContext
{
    public class SqlDbContext : IdentityDbContext<IdentityUser>
    // public class SqlDbContext : DbContext
    {
        public SqlDbContext(DbContextOptions<SqlDbContext> options) : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<ProductTag> ProductTags { get; set; }
        public DbSet<PushMail> PushMails { get; set; }
        public DbSet<Message> Messages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // 調用基類的 OnModelCreating 以配置 Identity 實體
            // { 這行程式碼確保了 Identity 系統所需的資料庫架構被正確設置，而不會被你後續的自訂配置覆蓋。}
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Product>()
                        .Property(p => p.Price)
                        .HasColumnType("decimal(18,2)"); // 設置精度 18，規模 2

            modelBuilder.Entity<ProductTag>()
                .HasKey(pt => new { pt.ProductId, pt.TagId });

            modelBuilder.Entity<ProductTag>()
                .HasOne(pt => pt.Product)
                .WithMany(p => p.ProductTags)
                .HasForeignKey(pt => pt.ProductId);

            modelBuilder.Entity<ProductTag>()
                .HasOne(pt => pt.Tag)
                .WithMany(t => t.ProductTags)
                .HasForeignKey(pt => pt.TagId);

            // 種子資料
            modelBuilder.Entity<Tag>().HasData(
                new Tag { Id = 1, Name = "麵包" },
                new Tag { Id = 2, Name = "甜點" },
                new Tag { Id = 3, Name = "季節限定" }
            );

            modelBuilder.Entity<Product>().HasData(
                new Product { Id = 1, Name = "法式雜糧長棍", Price = 100m, Specs = "500g", Description = "這是一款美味的雜糧長棍，使用頂級原料製作而成，口感濃郁綿密。", ImageUrl = "/images/p1.png" },
                new Product { Id = 2, Name = "牛角可頌", Price = 80m, Specs = "300g", Description = "酥脆的牛角可頌，完美搭配早餐或下午茶。", ImageUrl = "/images/p2.png" },
                new Product { Id = 3, Name = "雜糧吐司", Price = 80m, Specs = "300g", Description = "健康又美味的雜糧吐司，適合全家享用。", ImageUrl = "/images/p3.png" },
                new Product { Id = 4, Name = "鮮果鬆餅", Price = 80m, Specs = "300g", Description = "搭配新鮮水果的鬆餅，甜而不膩。", ImageUrl = "/images/p4.png" },
                new Product { Id = 5, Name = "莓果派", Price = 80m, Specs = "300g", Description = "酸甜可口的莓果派，下午茶最佳選擇。", ImageUrl = "/images/p5.png" },
                new Product { Id = 6, Name = "堅果塔", Price = 80m, Specs = "300g", Description = "香脆堅果搭配濃郁內餡，令人回味無窮。", ImageUrl = "/images/p6.png" }
            );

            modelBuilder.Entity<ProductTag>().HasData(
                new ProductTag { ProductId = 1, TagId = 1 }, // 法式雜糧長棍 -> 麵包
                new ProductTag { ProductId = 2, TagId = 1 }, // 牛角可頌 -> 麵包
                new ProductTag { ProductId = 3, TagId = 1 }, // 雜糧吐司 -> 麵包
                new ProductTag { ProductId = 4, TagId = 2 }, // 鮮果鬆餅 -> 甜點
                new ProductTag { ProductId = 5, TagId = 2 }, // 莓果派 -> 甜點
                new ProductTag { ProductId = 5, TagId = 3 }, // 莓果派 -> 季節限定
                new ProductTag { ProductId = 6, TagId = 2 }  // 堅果塔 -> 甜點
            );
        }

        /// <summary>
        /// 測試資料庫連線並列印狀態
        /// </summary>
        /// <param name="connectionString">資料庫連接字串</param>
        public void TestDatabaseConnection(string connectionString)
        {
            try
            {
                // 顯示連接字串
                Console.WriteLine($"Connection String: {connectionString}");

                // 測試資料庫連線
                if (Database.CanConnect())
                {
                    Console.WriteLine("Database connection successful!");
                    Console.WriteLine("資料庫連結成功");
                }
                else
                {
                    Console.WriteLine("Database connection failed!");
                    Console.WriteLine("資料庫連結失敗");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database connection failed: {ex.Message}");
            }
        }
    }

}