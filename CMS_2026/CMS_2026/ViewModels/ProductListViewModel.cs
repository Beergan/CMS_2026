using CMS_2026.Attributes;
using CMS_2026.Data.Entities;
using static CMS_2026.Common.InputControlType;

namespace CMS_2026.ViewModels
{
    [Component(
        Type = "Page_Template",
        ComptName = "en:Product list template|vi:Mẫu trang danh sách sản phẩm",
        PathPostfix = "/{0}",
        PageType = "list",
        NodeType = "product")]
    public class ProductListViewModel
    {
        public string? Title { get; set; }
        public string? MetaDescription { get; set; }
        public string? MetaKeywords { get; set; }
        public string? CategoryPath { get; set; }
        public int CurrentPage { get; set; } = 1;
        public long TotalPages { get; set; } = 1;

        // These are runtime data, not from config
        public List<PP_Product> Items { get; set; } = new();
        public List<PP_Product> Count { get; set; } = new();
        public List<PP_Category> Categories { get; set; } = new();
        public List<PP_Comment> Comments { get; set; } = new();

        [Field(Title = "Banner", Required = false, Control = Image)]
        public string? BannerProduct { get; set; }

        [Field(Title = "Banner quảng bá", Required = false, Control = Image)]
        public string? BannerPromote { get; set; }

        public static ProductListViewModel Default => new ProductListViewModel
        {
            Title = "Danh sách sản phẩm"
        };
    }
}

