using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CMS_2026.Data;

namespace CMS_2026.Data.Entities
{
    [Table("pp_config")]
    public class PP_Config : EntityBase
    {
        [Display(Name = "en:Language|vi:Ngôn ngữ")]
        [Required]
        [MaxLength(10)]
        public string LangId { get; set; } = string.Empty;

        [Display(Name = "Page ID")]
        public int PageId { get; set; }

        [Display(Name = "Config Key")]
        [Required]
        [MaxLength(100)]
        public string ConfigKey { get; set; } = string.Empty;

        [Display(Name = "JSON Content")]
        [Column(TypeName = "ntext")]
        public string? JsonContent { get; set; }
    }
}

