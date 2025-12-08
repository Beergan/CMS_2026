using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public async Task OnGetAsync(int? catId = null)
        {
            CatId = catId ?? 0;
            var query = await Db.GetListAsync<PP_Product>(t => t.LangId == LangIdCompose);

            if (catId.HasValue && catId.Value > 0)
            {
                Category = await Db.GetOneAsync<PP_Category>(catId.Value);
                query = query.Where(t => t.CategoryId == catId.Value).ToList();
            }

            Products = query
                .OrderByDescending(t => t.CreatedTime)
                .ToList();
        }

        public async Task<IActionResult> OnPostDeleteAsync([FromForm] int Id)
        {
            try
            {
                var item = await Db.GetOneAsync<PP_Product>(Id);
                if (item == null)
                {
                    return new JsonResult(new { success = false, message = "Không tìm thấy sản phẩm!" });
                }

                // Delete related variants
                var productVariantValues = await Db.GetListAsync<PP_productVariantValues>(x => x.Idproduct == Id);
                var productVariants = await Db.GetListAsync<PP_productvariants>(x => x.ProductIP == Id);
                var variantValues = await Db.GetListAsync<PP_variantValues>(x => x.Idproduct == Id);
                var variants = await Db.GetListAsync<PP_Variants>(x => x.Idproduct == Id);

                foreach (var pvv in productVariantValues)
                {
                    await Db.DeleteAsync<PP_productVariantValues>(pvv.Id);
                }

                foreach (var pv in productVariants)
                {
                    await Db.DeleteAsync<PP_productvariants>(pv.Id);
                }

                foreach (var vv in variantValues)
                {
                    await Db.DeleteAsync<PP_variantValues>(vv.Id);
                }

                foreach (var v in variants)
                {
                    await Db.DeleteAsync<PP_Variants>(v.Id);
                }

                await Db.DeleteAsync<PP_Product>(item.Id);
                return new JsonResult(new { success = true, message = $"Mục [{item.Title}] đã được xóa!" });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }
    }
}

