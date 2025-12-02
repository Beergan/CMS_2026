using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CMS_2026.Data;

namespace CMS_2026.Data.Entities
{
    [Table("pp_lang")]
    public class PP_Lang : EntityBase
    {
        [Display(Name = "en:LangId|vi:Mã ngôn ngữ")]
        [Key]
        public string LangId { get; set; } = string.Empty;

        [Display(Name = "en:Language name|vi:Tên ngôn ngữ")]
        public string Title { get; set; } = string.Empty;

        [Display(Name = "en:Date format|vi:Định dạng ngày")]
        public string? DateFormat { get; set; }

        [Display(Name = "en:Time format|vi:Định dạng giờ")]
        public string? TimeFormat { get; set; }

        [Display(Name = "en:Enabled|vi:Bật/tắt")]
        public bool Enabled { get; set; } = true;

        [Display(Name = "en:Default|vi:Mặc định")]
        public bool IsPrimary { get; set; } = false;

        // Alias for Title
        public string LangName => Title;
        
        // Alias for Enabled
        public string? Status => Enabled ? "ACTIVE" : "INACTIVE";
    }
}

