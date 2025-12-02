using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CMS_2026.Data;

namespace CMS_2026.Data.Entities
{
    [Table("pp_node")]
    public class PP_Node : EntityBase
    {
        [Display(Name = "en:Language|vi:Ngôn ngữ")]
        [Required]
        [MaxLength(10)]
        public string LangId { get; set; } = string.Empty;

        [Display(Name = "en:Category|vi:Chuyên mục")]
        public int? CategoryId { get; set; }

        [Display(Name = "en:Page template|vi:Mẫu hiển thị")]
        public int PageId { get; set; }

        [Display(Name = "en:Type|vi:Phân loại")]
        [MaxLength(50)]
        public string? NodeType { get; set; }

        [Display(Name = "en:Status|vi:Trạng thái")]
        [MaxLength(50)]
        public string? NodeStatus { get; set; }

        [Display(Name = "en:Title|vi:Tiêu đề")]
        [Required]
        [MaxLength(500)]
        public string Title { get; set; } = string.Empty;

        [Display(Name = "en:Path|vi:Đường dẫn")]
        [MaxLength(500)]
        public string? NodePath { get; set; }

        [Display(Name = "en:Featured|vi:Nổi bật")]
        public bool Featured { get; set; }

        [Display(Name = "en:Summary|vi:Tóm tắt")]
        [MaxLength(2000)]
        public string? Summary { get; set; }

        [Display(Name = "en:Content|vi:Nội dung")]
        [Column(TypeName = "ntext")]
        public string? Content { get; set; }

        [MaxLength(1000)]
        public string? MetaDescription { get; set; }

        [MaxLength(500)]
        public string? MetaKeywords { get; set; }

        [Display(Name = "en:Image|vi:Ảnh đại diện")]
        [MaxLength(1000)]
        public string? ImageUrl { get; set; }

        [MaxLength(500)]
        public string? listcat { get; set; }

        [NotMapped]
        public string? Breadcrumb { get; set; }

        [NotMapped]
        public int? PageIdItem { get; set; }

        [NotMapped]
        public string? CategoryName { get; set; }

        [NotMapped]
        public string? Status => NodeStatus;

        [NotMapped]
        public DateTime? UpdatedTime => UpdatedDate;

        public PP_Node SetBreadcrumb(string b)
        {
            this.Breadcrumb = b;
            return this;
        }
    }
}

