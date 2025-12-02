using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMS_2026.Data.Entities
{
    [Table("pp_role_claims")]
    public class PP_RoleClaims
    {
        [Key]
        public int Id { get; set; }

        public int RoleId { get; set; }

        [MaxLength(200)]
        public string? ClaimType { get; set; }

        public long ClaimValue { get; set; }
    }
}

