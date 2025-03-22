using System.ComponentModel.DataAnnotations;

namespace KoaLaDessertWeb.Models
{
    // 商品
    public class Product
    {
        [Key]
        public int Id { get; set; } // 主鍵
        public string ImageUrl { get; set; } // 商品圖片
        public string Name { get; set; } // 商品姓名
        public decimal Price { get; set; } // 商品價格
        public string Specs { get; set; } // 商品規格
        public string Description { get; set; } // 商品描述

        public List<ProductTag> ProductTags { get; set; } = new List<ProductTag>(); // 商品標籤
    }
    // 標籤
    public class Tag{
        public int Id { get; set; } // 主鍵
        public string Name { get; set; } // 標籤名稱
        public List<ProductTag> ProductTags { get; set; } = new List<ProductTag>(); // 商品標籤
    }
    // 商品標籤
    public class ProductTag{
        public int ProductId { get; set; } // 外鍵，指向 商品Id
        public Product Product { get; set; } // 商品
        public int TagId { get; set; } // 外鍵，指向 標籤Id
        public Tag Tag { get; set; } // 標籤
    }

}