using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CMS_2026.Data.Entities;
using CMS_2026.Services;

namespace CMS_2026.Pages.Admin.Lang
{
    public class CreateModel : BaseAdminPageModel
    {
        public CreateModel(IDataService dataService, RootService rootService, 
            Microsoft.Extensions.Caching.Memory.IMemoryCache cache, PermissionService permissionService)
            : base(dataService, rootService, cache, permissionService)
        {
        }

        public void OnGet()
        {
        }

        public IActionResult OnPost([FromForm] string LangId, [FromForm] string Title,
            [FromForm] string? DateFormat, [FromForm] string? TimeFormat, [FromForm] bool Enabled)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(LangId) || string.IsNullOrWhiteSpace(Title))
                {
                    return new JsonResult(new { success = false, message = "Vui lòng điền đầy đủ thông tin!" });
                }

                if (Db.GetList<PP_Lang>(t => t.LangId == LangId).Any())
                {
                    return new JsonResult(new { success = false, message = "Mã ngôn ngữ đã tồn tại!" });
                }

                var lang = new PP_Lang
                {
                    LangId = LangId,
                    Title = Title,
                    DateFormat = DateFormat ?? "dd/MM/yyyy",
                    TimeFormat = TimeFormat ?? "HH:mm",
                    Enabled = Enabled
                };

                Db.Insert(lang);
                Root.ReloadLangs();

                return new JsonResult(new { success = true, message = "Tạo ngôn ngữ thành công!", redirect = "/admin/lang" });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }
    }
}

