using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CMS_2026.Data;

namespace CMS_2026.Data.Entities
{
    [Table("pp_productvariantvalues")]
    public class PP_productVariantValues : EntityBase
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

        public int ProductVariantID { get; set; }

        public int? Idproduct { get; set; }

        [MaxLength(200)]
        public string? Name { get; set; }

        public int VariantID { get; set; }

        public int? ValueID { get; set; }
    }
}

