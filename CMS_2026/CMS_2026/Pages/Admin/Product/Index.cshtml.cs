using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CMS_2026.Data.Entities;
using CMS_2026.Services;

namespace CMS_2026.Pages.Admin.Product
{
    public class IndexModel : BaseAdminPageModel
    {
        public List<PP_Product> Products { get; set; } = new();
        public PP_Category? Category { get; set; }
        public int CatId { get; set; }

        public IndexModel(IDataService dataService, RootService rootService, 
            Microsoft.Extensions.Caching.Memory.IMemoryCache cache, PermissionService permissionService)
            : base(dataService, rootService, cache, permissionService)
        {
        }

        public void OnGet(int? catId = null)
        {
            CatId = catId ?? 0;
            var query = Db.GetList<PP_Product>(t => t.LangId == LangIdCompose);

            if (catId.HasValue && catId.Value > 0)
            {
                Category = Db.GetOne<PP_Category>(catId.Value);
                query = query.Where(t => t.CategoryId == catId.Value).ToList();
            }

            Products = query
                .OrderByDescending(t => t.CreatedTime)
                .ToList();
        }

        public IActionResult OnPostDelete([FromForm] int Id)
        {
            try
            {
                var item = Db.GetOne<PP_Product>(Id);
                if (item == null)
                {
                    return new JsonResult(new { success = false, message = "Không tìm thấy sản phẩm!" });
                }

                // Delete related variants
                var productVariantValues = Db.GetList<PP_productVariantValues>(x => x.Idproduct == Id);
                var productVariants = Db.GetList<PP_productvariants>(x => x.ProductIP == Id);
                var variantValues = Db.GetList<PP_variantValues>(x => x.Idproduct == Id);
                var variants = Db.GetList<PP_Variants>(x => x.Idproduct == Id);

                foreach (var pvv in productVariantValues)
                {
                    Db.Delete<PP_productVariantValues>(pvv.Id);
                }

                foreach (var pv in productVariants)
                {
                    Db.Delete<PP_productvariants>(pv.Id);
                }

                foreach (var vv in variantValues)
                {
                    Db.Delete<PP_variantValues>(vv.Id);
                }

                foreach (var v in variants)
                {
                    Db.Delete<PP_Variants>(v.Id);
                }

                Db.Delete<PP_Product>(item.Id);
                return new JsonResult(new { success = true, message = $"Mục [{item.Title}] đã được xóa!" });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }
    }
}

