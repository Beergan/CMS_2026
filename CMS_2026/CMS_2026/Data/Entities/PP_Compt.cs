using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CMS_2026.Data;

namespace CMS_2026.Data.Entities
{
    [Table("pp_compt")]
    public class PP_Compt : EntityBase
    {
        [Display(Name = "en:Key|vi:Khóa")]
        [Required]
        [MaxLength(100)]
        public string ComptKey { get; set; } = string.Empty;

        [Display(Name = "en:Type|vi:Phân loại")]
        [MaxLength(50)]
        public string? ComptType { get; set; }

        [Display(Name = "en:Component name|vi:Tên component")]
        [MaxLength(500)]
        public string? ComptName { get; set; }

        [MaxLength(50)]
        public string? NodeType { get; set; }

        [MaxLength(50)]
        public string? PageType { get; set; }

        [Display(Name = "en:Path postfix|vi:Hậu tố đường dẫn")]
        [MaxLength(100)]
        public string? PathPostfix { get; set; }

        [Display(Name = "en:Schema|vi:Lược đồ")]
        [Column(TypeName = "ntext")]
        public string? JsonSchema { get; set; }

        [Display(Name = "en:Default|vi:Mặc định")]
        [Column(TypeName = "ntext")]
        public string? JsonDefault { get; set; }
    }

    [NotMapped]
    public class ComptWithLangs : PP_Compt
    {
        [Display(Name = "en:Langs|vi:Ngôn ngữ")]
        public string? Langs { get; set; }
    }
}

