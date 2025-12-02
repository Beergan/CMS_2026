using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMS_2026.Data.Entities
{
    [Table("pp_json")]
    public class PP_Json
    {
        [Key]
        [MaxLength(100)]
        public string JsonKey { get; set; } = string.Empty;

        [Column(TypeName = "ntext")]
        public string? JsonContent { get; set; }
    }
}

