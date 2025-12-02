using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMS_2026.Data.Entities
{
    [Table("pp_userroles")]
    public class PP_UserRoles
    {
        [Key]
        public int Id { get; set; }

        public int UserId { get; set; }

        public int RoleId { get; set; }
    }
}

