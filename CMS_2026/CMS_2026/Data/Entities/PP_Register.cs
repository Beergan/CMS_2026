using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CMS_2026.Data;

namespace CMS_2026.Data.Entities
{
    [Table("pp_register")]
    public class PP_Register : EntityBase
    {
        [MaxLength(50)]
        public string? Status { get; set; }

        [MaxLength(200)]
        public string? Name { get; set; }

        [MaxLength(200)]
        public string? Email { get; set; }

        [MaxLength(50)]
        public string? Phone { get; set; }

        [Column(TypeName = "ntext")]
        public string? Message { get; set; }

        public bool Active { get; set; }

        [MaxLength(500)]
        public string? Address { get; set; }

        [MaxLength(500)]
        public string? PASSWORD { get; set; }

        public int? Idorder { get; set; }

        [Column(TypeName = "ntext")]
        public string? ProcessNote { get; set; }
    }
}

