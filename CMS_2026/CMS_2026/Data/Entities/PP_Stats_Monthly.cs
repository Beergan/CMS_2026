using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMS_2026.Data.Entities
{
    [Table("pp_stats_monthly")]
    public class PP_Stats_Monthly
    {
        [Key]
        public int Month { get; set; }

        public int VisitCount { get; set; }

        public int OrderCount { get; set; }
    }
}

