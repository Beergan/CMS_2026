using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMS_2026.Data.Entities
{
    [Table("pp_stats_url")]
    public class PP_Stats_Url
    {
        [Key]
        [MaxLength(1000)]
        public string Url { get; set; } = string.Empty;

        public int VisitCount { get; set; }
    }
}

