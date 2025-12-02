using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CMS_2026.Data;

namespace CMS_2026.Data.Entities
{
    [Table("pp_evaluation")]
    public class PP_Evaluation : EntityBase
    {
        [MaxLength(50)]
        public string? Status { get; set; }

        [MaxLength(200)]
        public string? FirstName { get; set; }

        [MaxLength(200)]
        public string? LastName { get; set; }

        [MaxLength(200)]
        public string? Email { get; set; }

        [MaxLength(50)]
        public string? Phone { get; set; }

        [Column(TypeName = "ntext")]
        public string? Message { get; set; }

        [Column(TypeName = "ntext")]
        public string? ProcessNote { get; set; }
    }
}

