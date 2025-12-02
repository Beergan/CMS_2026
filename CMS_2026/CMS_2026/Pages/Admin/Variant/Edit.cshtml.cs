using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CMS_2026.Data.Entities;
using CMS_2026.Services;
using CMS_2026.Pages.Admin;
using CMS_2026.Utils;

namespace CMS_2026.Pages.Admin.Variant
{
    public class EditModel : BaseAdminPageModel
    {
        public PP_productvariants? Variant { get; set; }
        public PP_Product? Product { get; set; }

        public EditModel(IDataService dataService, RootService rootService, 
            Microsoft.Extensions.Caching.Memory.IMemoryCache cache, PermissionService permissionService)
            : base(dataService, rootService, cache, permissionService)
        {
        }

        public IActionResult OnGet(int? id)
        {
            if (!id.HasValue)
            {
                return Redirect("/admin/product");
            }

            Variant = Db.GetOne<PP_productvariants>(id.Value);
            if (Variant == null)
            {
                return Redirect("/admin/product");
            }

            Product = Db.GetOne<PP_Product>(Variant.ProductIP);
            return Page();
        }

        public IActionResult OnPost([FromForm] int Id, [FromForm] string? IDSKD,
            [FromForm] decimal Price, [FromForm] decimal Discount, [FromForm] int Stock,
            [FromForm] string? Image)
        {
            try
            {
                var variant = Db.GetOne<PP_productvariants>(Id);
                if (variant == null)
                {
                    return new JsonResult(new { success = false, message = "Biến thể không tồn tại!" });
                }

                if (Stock <= 0)
                {
                    return new JsonResult(new { success = false, message = "Trọng lượng phải lớn hơn 0!" });
                }

                variant.IDSKD = IDSKD?.NullIfWhiteSpace();
                variant.Price = Price;
                variant.Discount = Discount;
                variant.Stock = Stock;
                variant.Image = Image?.NullIfWhiteSpace();

                Db.Update(variant);
                Db.SaveChanges();
                Root.ClearCache();

                return new JsonResult(new { success = true, message = "Cập nhật biến thể thành công!", redirect = "/admin/product" });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }
    }
}

