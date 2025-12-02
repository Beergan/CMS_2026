using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CMS_2026.Data.Entities;
using CMS_2026.Services;
using CMS_2026.Pages.Admin;
using CMS_2026.Utils;

namespace CMS_2026.Pages.Admin.Variant
{
    public class CreateModel : BaseAdminPageModel
    {
        public PP_Product? Product { get; set; }
        public Dictionary<string, string> GroupSelector { get; set; } = new();

        public CreateModel(IDataService dataService, RootService rootService, 
            Microsoft.Extensions.Caching.Memory.IMemoryCache cache, PermissionService permissionService)
            : base(dataService, rootService, cache, permissionService)
        {
        }

        public IActionResult OnGet(int? productId)
        {
            if (!productId.HasValue)
            {
                return Redirect("/admin/product");
            }

            Product = Db.GetOne<PP_Product>(productId.Value);
            if (Product == null)
            {
                return Redirect("/admin/product");
            }

            GroupSelector = GetGroupSelector(LangIdCompose, "product");
            return Page();
        }

        public IActionResult OnPost([FromForm] int ProductId, [FromForm] string? IDSKD,
            [FromForm] decimal Price, [FromForm] decimal Discount, [FromForm] int Stock,
            [FromForm] string? Image)
        {
            try
            {
                var product = Db.GetOne<PP_Product>(ProductId);
                if (product == null)
                {
                    return new JsonResult(new { success = false, message = "Sản phẩm không tồn tại!" });
                }

                if (Stock <= 0)
                {
                    return new JsonResult(new { success = false, message = "Trọng lượng phải lớn hơn 0!" });
                }

                var variant = new PP_productvariants
                {
                    LangId = LangIdCompose,
                    ProductIP = ProductId,
                    IDSKD = IDSKD?.NullIfWhiteSpace(),
                    Price = Price,
                    Discount = Discount,
                    Stock = Stock,
                    Image = Image?.NullIfWhiteSpace(),
                    CreatedTime = DateTime.Now
                };

                Db.Insert(variant);
                Db.SaveChanges();
                Root.ClearCache();

                return new JsonResult(new { success = true, message = "Tạo biến thể thành công!", redirect = "/admin/product" });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }
    }
}

