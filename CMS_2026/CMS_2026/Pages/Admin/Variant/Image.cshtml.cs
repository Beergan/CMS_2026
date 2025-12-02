using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CMS_2026.Data.Entities;
using CMS_2026.Services;
using CMS_2026.Pages.Admin;
using System.Text.Json;

namespace CMS_2026.Pages.Admin.Variant
{
    public class ImageModel : BaseAdminPageModel
    {
        public PP_productvariants? Variant { get; set; }
        public List<string> ImageUrls { get; set; } = new();

        public ImageModel(IDataService dataService, RootService rootService, 
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

            // Parse images from JSON if exists
            if (!string.IsNullOrEmpty(Variant.Image))
            {
                ImageUrls.Add(Variant.Image);
            }

            return Page();
        }

        public IActionResult OnPostAppend([FromForm] int Id, [FromForm] string ImageUrl)
        {
            try
            {
                var variant = Db.GetOne<PP_productvariants>(Id);
                if (variant == null)
                {
                    return new JsonResult(new { success = false, message = "Biến thể không tồn tại!" });
                }

                if (string.IsNullOrEmpty(ImageUrl))
                {
                    return new JsonResult(new { success = false, message = "URL hình ảnh không được để trống!" });
                }

                variant.Image = ImageUrl;
                Db.Update(variant);
                Db.SaveChanges();
                Root.ClearCache();

                return new JsonResult(new { success = true, message = "Thêm hình thành công!" });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }

        public IActionResult OnPostRemove([FromForm] int Id)
        {
            try
            {
                var variant = Db.GetOne<PP_productvariants>(Id);
                if (variant == null)
                {
                    return new JsonResult(new { success = false, message = "Biến thể không tồn tại!" });
                }

                variant.Image = null;
                Db.Update(variant);
                Db.SaveChanges();
                Root.ClearCache();

                return new JsonResult(new { success = true, message = "Xóa hình thành công!" });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }
    }
}

