using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CMS_2026.Data;

namespace CMS_2026.Data.Entities
{
    [Table("pp_product")]
    public class PP_Product : EntityBase
    {
        [Display(Name = "en:Language|vi:Ngôn ngữ")]
        [Required]
        [MaxLength(10)]
        public string LangId { get; set; } = string.Empty;

        [Display(Name = "en:Page template|vi:Mẫu hiển thị")]
        public int PageId { get; set; }

        [Display(Name = "en:Category|vi:Chuyên mục")]
        public int? CategoryId { get; set; }

        [Display(Name = "en:Type|vi:Phân loại")]
        [MaxLength(50)]
        public string? NodeType { get; set; }

        [Display(Name = "en:Status|vi:Trạng thái")]
        [MaxLength(50)]
        public string? NodeStatus { get; set; }

        [Display(Name = "en:Title|vi:Tiêu đề")]
        [Required]
        [MaxLength(500)]
        public string Title { get; set; } = string.Empty;

        [Display(Name = "en:Path|vi:Đường dẫn")]
        [MaxLength(500)]
        public string? NodePath { get; set; }

        [Display(Name = "en:Product name|vi:Tên nhãn hiệu")]
        [MaxLength(200)]
        public string? Brand { get; set; }

        [Display(Name = "en:Mô tả sản phẩm|vi:Mô tả sản phẩm")]
        [Column(TypeName = "ntext")]
        public string? Content { get; set; }

        [Display(Name = "en:Mô tả sản phẩm|vi:Thông tin sản phẩm")]
        [Column(TypeName = "ntext")]
        public string? Des { get; set; }

        [Display(Name = "en:Mô tả sản phẩm|vi:Thông tin bổ sung")]
        [Column(TypeName = "ntext")]
        public string? Note { get; set; }

        [Display(Name = "en:Attributes enabled|vi: Bật/tắt thuộc tính phân loại")]
        public bool AttrbEnabled { get; set; }

        [Display(Name = "en:Attribute name|vi:Tên thuộc tính")]
        [MaxLength(200)]
        public string? AttrbName { get; set; }

        [Display(Name = "en:Attribute values|vi:Giá trị thuộc tính")]
        [Column(TypeName = "ntext")]
        public string? AttrbValues { get; set; }

        [Display(Name = "en:Price|vi:Giá")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [Display(Name = "en:Promotion price|vi:Giá khuyến mãi")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal? PromotionPrice { get; set; }

        [Display(Name = "en:Promotion label|vi:Nhãn khuyến mãi")]
        [MaxLength(200)]
        public string? PromotionLabel { get; set; }

        [Display(Name = "en:Promotion enabled|vi:Kích hoạt khuyến mãi")]
        public bool PromotionEnabled { get; set; }

        [Display(Name = "en:Promotion expiration|vi:Thời hạn khuyến mãi")]
        public DateTime? PromotionExpiration { get; set; }

        [Display(Name = "en:Best seller|vi:Sản phẩm bán chạy")]
        public bool BestSeller { get; set; }

        [Display(Name = "en:Product code|vi:Mã sản phẩm")]
        [MaxLength(100)]
        public string? ProductCode { get; set; }

        [Display(Name = "en:Stock qty|vi:Số lượng tồn")]
        public int StockQty { get; set; }

        [Display(Name = "en:Product weight|vi:Trọng lượng hàng", Prompt = "Tính bằng gram")]
        public int Weight { get; set; }

        [Display(Name = "en:View counter|vi:Lượt xem")]
        public int ViewCounter { get; set; }

        [Display(Name = "en:Sold counter|vi:Lượt bán")]
        public int SoldCounter { get; set; }

        [MaxLength(1000)]
        public string? MetaDescription { get; set; }

        [MaxLength(500)]
        public string? MetaKeywords { get; set; }

        [Display(Name = "en:Image|vi:Ảnh đại diện")]
        [MaxLength(1000)]
        public string? ImageUrl { get; set; }

        [Column(TypeName = "ntext")]
        public string? ImagesJson { get; set; }

        [MaxLength(500)]
        public string? listcat { get; set; }

        [NotMapped]
        public string? Breadcrumb { get; set; }

        [NotMapped]
        public int? PageIdItem { get; set; }

        [NotMapped]
        public string? CategoryName { get; set; }

        [NotMapped]
        public string? Status => NodeStatus;

        [NotMapped]
        public DateTime? UpdatedTime => UpdatedDate;

        public int View { get; set; }  
        public PP_Product SetBreadcrumb(string b)
        {
            this.Breadcrumb = b;
            return this;
        }
    }
}

