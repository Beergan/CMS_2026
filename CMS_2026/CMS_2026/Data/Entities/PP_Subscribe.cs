using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CMS_2026.Data;

namespace CMS_2026.Data.Entities
{
    [Table("pp_subscribe")]
    public class PP_Subscribe : EntityBase
    {
        [MaxLength(200)]
        public string? Email { get; set; }

        public DateTime SubscribeDate { get; set; } = DateTime.Now;

        [MaxLength(50)]
        public string? Ip { get; set; }

        [MaxLength(50)]
        public string? Status { get; set; }
    }
}

