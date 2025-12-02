using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CMS_2026.Data;

namespace CMS_2026.Data.Entities
{
    [Table("pp_productvariants")]
    public class PP_productvariants : EntityBase
    {
        [Display(Name = "en:Languague|vi:Ngôn ngữ")]
        [MaxLength(10)]
        public string? LangId { get; set; }

        [Display(Name = "en:Category|vi:Chuyên mục")]
        public int? ParentId { get; set; }

        [Display(Name = "en:List template|vi:Mẫu hiển thị danh sách")]
        public int PageId { get; set; }

        [Display(Name = "en:Item template|vi:Mẫu hiển thị phần tử")]
        public int PageIdItem { get; set; }

        [Display(Name = "en:Type|vi:Phân loại")]
        [MaxLength(50)]
        public string? NodeType { get; set; }

        public int ProductIP { get; set; }

        [MaxLength(100)]
        public string? IDSKD { get; set; }

        [MaxLength(500)]
        public string? Title { get; set; }

        [MaxLength(1000)]
        public string? Image { get; set; }

        [MaxLength(200)]
        public string? Name { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        public int ValueID { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Discount { get; set; }

        public int Stock { get; set; }

        public int VariantID { get; set; }
    }
}

