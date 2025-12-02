using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CMS_2026.Data.Entities;
using CMS_2026.Services;
using CMS_2026.Pages.Admin;
using CMS_2026.Utils;

namespace CMS_2026.Pages.Admin.Category
{
    public class ImageModel : BaseAdminPageModel
    {
        public PP_Category? Category { get; set; }

        public ImageModel(IDataService dataService, RootService rootService, 
            Microsoft.Extensions.Caching.Memory.IMemoryCache cache, PermissionService permissionService)
            : base(dataService, rootService, cache, permissionService)
        {
        }

        public IActionResult OnGet(int? id)
        {
            if (!id.HasValue)
            {
                return Redirect("/admin/category");
            }

            Category = Db.GetOne<PP_Category>(id.Value);
            if (Category == null)
            {
                return Redirect("/admin/category");
            }

            return Page();
        }

        public IActionResult OnPost([FromForm] int Id, [FromForm] string ImageUrl)
        {
            try
            {
                var category = Db.GetOne<PP_Category>(Id);
                if (category == null)
                {
                    return new JsonResult(new { success = false, message = "Chuyên mục không tồn tại!" });
                }

                category.ImageUrl = ImageUrl.NullIfWhiteSpace();
                Db.Update(category);
                Db.SaveChanges();
                Root.ClearCache();

                return new JsonResult(new { success = true, message = "Cập nhật hình ảnh thành công!" });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }
    }
}

