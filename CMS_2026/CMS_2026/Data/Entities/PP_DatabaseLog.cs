using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CMS_2026.Data;

namespace CMS_2026.Data.Entities
{
    [Table("pp_database_log")]
    public class PP_DatabaseLog : EntityBase
    {
        [Required]
        [MaxLength(20)]
        public string Action { get; set; } = string.Empty;
        [Required]
        [MaxLength(100)]
        public string EntityType { get; set; } = string.Empty;
        public int? EntityId { get; set; }
        [MaxLength(100)]
        public string? UserId { get; set; }
        public int? IdUser { get; set; }
        [MaxLength(200)]
        public string? DisplayName { get; set; }

        [MaxLength(50)]
        public string? IpAddress { get; set; }

        [MaxLength(4000)]
        public string? Description { get; set; }
        public DateTime LogTime { get; set; } = DateTime.Now;
    }
}

