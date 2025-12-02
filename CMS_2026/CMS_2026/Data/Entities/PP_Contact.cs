using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CMS_2026.Data;

namespace CMS_2026.Data.Entities
{
    [Table("pp_contact")]
    public class PP_Contact : EntityBase
    {
        [MaxLength(50)]
        public string? Status { get; set; }

        [MaxLength(200)]
        public string? Name { get; set; }

        [MaxLength(1000)]
        public string? Message { get; set; }

        [MaxLength(200)]
        public string? Email { get; set; }

        [MaxLength(500)]
        public string? Address { get; set; }

        [MaxLength(50)]
        public string? Phone { get; set; }

        [Column(TypeName = "ntext")]
        public string? ProcessNote { get; set; }
    }
}

