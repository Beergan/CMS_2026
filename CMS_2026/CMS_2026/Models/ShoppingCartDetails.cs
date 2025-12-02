using CMS_2026.Data.Entities;

namespace CMS_2026.Models
{
    public class ShoppingCartDetails
    {
        public PP_Product Product { get; set; } = null!;
        public string? Variation { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }

    public class OrderLine
    {
        public int ID { get; set; }
        public string? Brand { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Variation { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal RowTotal { get; set; }
        public int Weight { get; set; }
        public bool PromotionEnabled { get; set; }
    }
}

