using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CMS_2026.Data;

namespace CMS_2026.Data.Entities
{
    [Table("pp_order")]
    public class PP_Order : EntityBase
    {
        [MaxLength(100)]
        public string? OrderCode { get; set; }

        [Display(Name = "en:Status|vi:Trạng thái")]
        [MaxLength(50)]
        public string? OrderStatus { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalWeight { get; set; }

        [Display(Name = "en:Ship fee|vi:Phí ship")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal ShipFee { get; set; }

        [Display(Name = "Tổng tiền hàng")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal SubTotalAmount { get; set; }

        [Display(Name = "Tổng cộng")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        [Display(Name = "Thanh toán")]
        [MaxLength(50)]
        public string? PayMethod { get; set; }

        [MaxLength(200)]
        public string? Name { get; set; }

        [MaxLength(50)]
        public string? Phone { get; set; }

        [MaxLength(200)]
        public string? Email { get; set; }

        [MaxLength(1000)]
        public string? DeliveryAddress { get; set; }

        [MaxLength(100)]
        public string? Province { get; set; }

        [MaxLength(100)]
        public string? District { get; set; }

        [MaxLength(100)]
        public string? Ward { get; set; }

        [Column(TypeName = "ntext")]
        public string? Note { get; set; }

        [Column(TypeName = "ntext")]
        public string? JsonData { get; set; }

        [Column(TypeName = "ntext")]
        public string? ReasonForCancel { get; set; }

        public int GhtkStatusId { get; set; }

        [MaxLength(200)]
        public string? GhtkLabel { get; set; }

        public int GhtkFee { get; set; }

        [MaxLength(100)]
        public string? GhTkEstimatedPickTime { get; set; }

        [MaxLength(100)]
        public string? GhtkEstimatedDeliverTime { get; set; }

        [MaxLength(50)]
        public string? IpAddress { get; set; }

        // Alias for Name
        public string? CustomerName => Name;
    }
}

