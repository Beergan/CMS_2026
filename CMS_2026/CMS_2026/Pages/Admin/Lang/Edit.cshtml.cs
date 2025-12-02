using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CMS_2026.Data.Entities;
using CMS_2026.Services;

namespace CMS_2026.Pages.Admin.Lang
{
    public class EditModel : BaseAdminPageModel
    {
        public PP_Lang? Lang { get; set; }

        public EditModel(IDataService dataService, RootService rootService, 
            Microsoft.Extensions.Caching.Memory.IMemoryCache cache, PermissionService permissionService)
            : base(dataService, rootService, cache, permissionService)
        {
        }

        public IActionResult OnGet(int? id)
        {
            if (!id.HasValue)
            {
                return Redirect("/admin/lang");
            }

            Lang = Db.GetOne<PP_Lang>(id.Value);
            if (Lang == null)
            {
                return Redirect("/admin/lang");
            }

            return Page();
        }

        public IActionResult OnPost([FromForm] int Id, [FromForm] string Title,
            [FromForm] string? DateFormat, [FromForm] string? TimeFormat, [FromForm] bool Enabled)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Title))
                {
                    return new JsonResult(new { success = false, message = "Tên ngôn ngữ không được để trống!" });
                }

                var lang = Db.GetOne<PP_Lang>(Id);
                if (lang == null)
                {
                    return new JsonResult(new { success = false, message = "Không tìm thấy ngôn ngữ!" });
                }

                lang.Title = Title;
                lang.DateFormat = DateFormat ?? "dd/MM/yyyy";
                lang.TimeFormat = TimeFormat ?? "HH:mm";
                lang.Enabled = Enabled;

                Db.Update(lang);
                Root.ReloadLangs();

                return new JsonResult(new { success = true, message = "Cập nhật thành công!", redirect = "/admin/lang" });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }
    }
}

