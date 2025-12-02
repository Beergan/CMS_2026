namespace CMS_2026.ViewModels
{
    /// <summary>
    /// View model for product display in components (like Home, ProductList)
    /// Mirrors the old VM_Product structure from the legacy codebase
    /// </summary>
    public class VM_Product
    {
        public string? JsonContent { get; set; }
        public string? Title { get; set; }
        public string? Path { get; set; }
        public string? Summary { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime CreatedTime { get; set; }
        public int? CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public string? CategoryPath { get; set; }
        public bool BestSeller { get; set; }
        public decimal Price { get; set; }
        public int idnode { get; set; }
        public string? NodePath { get; set; }
        public decimal? PromotionPrice { get; set; }
        public bool PromotionEnabled { get; set; }
        public string? AttrbValues { get; set; }
        public bool AttrbEnabled { get; set; }
    }
}
