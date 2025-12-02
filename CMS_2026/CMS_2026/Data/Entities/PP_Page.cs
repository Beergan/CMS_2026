using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CMS_2026.Data;

namespace CMS_2026.Data.Entities
{
    [Table("pp_page")]
    public class PP_Page : EntityBase
    {
        [Display(Name = "en:Language|vi:Ngôn ngữ")]
        [Required]
        [MaxLength(10)]
        public string LangId { get; set; } = string.Empty;

        [Display(Name = "en:Type|vi:Phân loại")]
        [MaxLength(50)]
        public string? PageType { get; set; }

        [Display(Name = "en:Type|vi:Phân loại")]
        [MaxLength(50)]
        public string? NodeType { get; set; }

        [Display(Name = "en:Status|vi:Trạng thái")]
        [MaxLength(50)]
        public string? PageStatus { get; set; }

        [Display(Name = "en:Title|vi:Tiêu đề")]
        [Required]
        [MaxLength(500)]
        public string Title { get; set; } = string.Empty;

        [Display(Name = "en:Path|vi:Đường dẫn")]
        [Required]
        [MaxLength(500)]
        public string PathPattern { get; set; } = string.Empty;

        [Display(Name = "en:Template page|vi:Mẫu hiển thị")]
        [Required]
        [MaxLength(100)]
        public string ComptKey { get; set; } = string.Empty;

        [Display(Name = "en:Template page|vi:Mẫu hiển thị")]
        [MaxLength(500)]
        public string? ComptName { get; set; }

        [Display(Name = "Meta Description")]
        [MaxLength(1000)]
        public string? MetaDescription { get; set; }

        [Display(Name = "Meta Keywords")]
        [MaxLength(500)]
        public string? MetaKeywords { get; set; }

        [NotMapped]
        public string? Status => PageStatus;

        [NotMapped]
        public string? NodePath => PathPattern;

        [NotMapped]
        public DateTime? UpdatedTime => UpdatedDate;
    }
}

