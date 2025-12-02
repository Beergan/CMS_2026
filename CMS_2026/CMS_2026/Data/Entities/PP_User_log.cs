using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CMS_2026.Data;

namespace CMS_2026.Data.Entities
{
    [Table("pp_user_log")]
    public class PP_User_log : EntityBase
    {
        [MaxLength(200)]
        public string? Surname { get; set; }

        [MaxLength(200)]
        public string? Name { get; set; }

        [MaxLength(200)]
        public string? Email { get; set; }

        [MaxLength(500)]
        public string? Password { get; set; }

        [MaxLength(200)]
        public string? DisplayName { get; set; }
    }
}

