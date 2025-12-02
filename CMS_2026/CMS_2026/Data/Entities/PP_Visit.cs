using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CMS_2026.Data;

namespace CMS_2026.Data.Entities
{
    [Table("pp_visit")]
    public class PP_Visit : EntityBase
    {
        [MaxLength(100)]
        public string? SessionId { get; set; }

        [Display(Name = "en:Time|vi:Thời gian")]
        public int Date { get; set; }

        [NotMapped]
        public long FirstTick { get; set; }

        [NotMapped]
        public long LastTick { get; set; } = 0L;

        [NotMapped]
        public long HeartBeat { get; set; } = 0L;

        [NotMapped]
        public List<KeyValuePair<string, long>>? Urls { get; set; }

        [MaxLength(1000)]
        public string? LastUrl { get; set; }

        [Display(Name = "en:Country|vi:Quốc gia")]
        [MaxLength(100)]
        public string? Country { get; set; }

        [Display(Name = "Referer")]
        [MaxLength(1000)]
        public string? Referer { get; set; }

        [Display(Name = "en:Device|vi:Thiết bị")]
        [MaxLength(100)]
        public string? Device { get; set; }

        [Display(Name = "en:Browser|vi:Trình duyệt")]
        [MaxLength(200)]
        public string? Browser { get; set; }

        [Display(Name = "en:Ip|vi:Địa chỉ ip")]
        [MaxLength(50)]
        public string? Ip { get; set; }

        public int ClickCount { get; set; }

        [Display(Name = "en:Make order|vi:Đặt hàng")]
        public bool MakeOrder { get; set; }

        public int StayTime { get; set; }

        [Column(TypeName = "ntext")]
        public string? JsonDetails { get; set; }
    }
}

