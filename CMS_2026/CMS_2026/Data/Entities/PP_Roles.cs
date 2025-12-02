using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMS_2026.Data.Entities
{
    [Table("pp_roles")]
    public class PP_Roles
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "en:Group name|vi:Tên nhóm")]
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? NormalizedName { get; set; }

        public int ConcurrencyStamp { get; set; }

        // Aliases for compatibility
        public string RoleName => Name;
        public string? Description { get; set; }
    }
}

