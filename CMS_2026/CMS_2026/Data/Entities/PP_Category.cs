using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CMS_2026.Data;

namespace CMS_2026.Data.Entities
{
    [Table("pp_category")]
    public class PP_Category : EntityBase
    {
        [Display(Name = "en:Languague|vi:Ngôn ngữ")]
        [Required]
        [MaxLength(10)]
        public string LangId { get; set; } = string.Empty;

        [Display(Name = "en:Category|vi:Chuyên mục")]
        public int? ParentId { get; set; }

        [Display(Name = "en:List template|vi:Mẫu hiển thị danh sách")]
        public int PageId { get; set; }

        [Display(Name = "en:Item template|vi:Mẫu hiển thị phần tử")]
        public int PageIdItem { get; set; }

        [Display(Name = "en:Type|vi:Phân loại")]
        [MaxLength(50)]
        public string? NodeType { get; set; }

        public int CategoryLevel { get; set; }

        [Display(Name = "en:Title|vi:Tiêu đề")]
        [Required]
        [MaxLength(500)]
        public string Title { get; set; } = string.Empty;

        [Display(Name = "en:Breadcrumb|vi:Breadcrumb")]
        [MaxLength(1000)]
        public string? Breadcrumb { get; set; }

        [Display(Name = "en:Category path|vi:Đường dẫn")]
        [MaxLength(500)]
        public string? CategoryPath { get; set; }

        [Display(Name = "en:Image|vi:Ảnh đại diện")]
        [MaxLength(1000)]
        public string? ImageUrl { get; set; }

        [Display(Name = "Meta Description")]
        [MaxLength(1000)]
        public string? MetaDescription { get; set; }

        [MaxLength(500)]
        public string? MetaKeywords { get; set; }
    }
}

