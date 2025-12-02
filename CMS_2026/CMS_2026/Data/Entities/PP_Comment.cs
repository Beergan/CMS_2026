using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CMS_2026.Data;

namespace CMS_2026.Data.Entities
{
    [Table("pp_comment")]
    public class PP_Comment : EntityBase
    {
        [MaxLength(50)]
        public string? Status { get; set; }

        [MaxLength(200)]
        public string? Name { get; set; }

        [MaxLength(200)]
        public string? Email { get; set; }

        [Column(TypeName = "ntext")]
        public string? Comment { get; set; }

        // Alias for Comment
        public string? Message => Comment;

        [Column(TypeName = "ntext")]
        public string? ProcessNote { get; set; }

        [MaxLength(50)]
        public string? notetype { get; set; }

        public int idproduct { get; set; }

        public int iduser { get; set; }

        public int star { get; set; }
    }
}

