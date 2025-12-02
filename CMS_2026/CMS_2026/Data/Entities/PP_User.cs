using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CMS_2026.Data;

namespace CMS_2026.Data.Entities
{
    [Table("pp_user")]
    public class PP_User : EntityBase
    {
        [Display(Name = "en:Full name|vi:Họ và tên")]
        [MaxLength(200)]
        public string? DisplayName { get; set; }

        [Display(Name = "en:Email|vi:Email")]
        [MaxLength(200)]
        public string? Email { get; set; }

        [Display(Name = "en:User id|vi:Tên tài khản")]
        [Required]
        [MaxLength(100)]
        public string UserId { get; set; } = string.Empty;

        [Display(Name = "en:Password|vi:Mật khẩu")]
        [Required]
        [MaxLength(500)]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "en:Enabled|vi:Kích hoạt")]
        public bool Enabled { get; set; } = true;
    }
}

