using CMS_2026.Attributes;
using CMS_2026.Data.Entities;
using static CMS_2026.Common.InputControlType;

namespace CMS_2026.ViewModels
{
    [Component(
        Type = "Page_Template",
        ComptName = "en:Blog list template|vi:Mẫu trang danh sách Blog",
        PathPostfix = "/{0}",
        PageType = "list",
        NodeType = "post")]
    public class BlogListViewModel
    {
        public string? Title { get; set; }
        public string? MetaDescription { get; set; }
        public string? MetaKeywords { get; set; }
        public string? CategoryPath { get; set; }
        public int CurrentPage { get; set; } = 1;
        public long TotalPages { get; set; } = 1;
        public string? Banner { get; set; }

        // These are runtime data, not from config
        public List<PP_Node> Items { get; set; } = new();
        public List<PP_Node> Count { get; set; } = new();
        public List<PP_Node> RecentItems { get; set; } = new();
        public List<PP_Category> cat { get; set; } = new();

        [Field(Title = "Banner", Required = false, Control = Image)]
        public string? BannerBlog { get; set; }

        public static BlogListViewModel Default => new BlogListViewModel
        {
            Title = "Danh sách blog"
        };
    }
}

