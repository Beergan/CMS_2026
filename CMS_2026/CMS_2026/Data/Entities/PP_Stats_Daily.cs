using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMS_2026.Data.Entities
{
    [Table("pp_stats_daily")]
    public class PP_Stats_Daily
    {
        [Key]
        public int Date { get; set; }

        public int VisitCount { get; set; }

        public int OrderCount { get; set; }
    }
}

