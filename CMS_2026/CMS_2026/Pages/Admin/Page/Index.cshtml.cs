using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CMS_2026.Data.Entities;
using CMS_2026.Services;

namespace CMS_2026.Pages.Admin.Page
{
    public class IndexModel : BaseAdminPageModel
    {
        public List<PP_Page> Pages { get; set; } = new();

        public IndexModel(IDataService dataService, RootService rootService, 
            Microsoft.Extensions.Caching.Memory.IMemoryCache cache, PermissionService permissionService)
            : base(dataService, rootService, cache, permissionService)
        {
        }

        public void OnGet()
        {
            Pages = Db.GetList<PP_Page>(t => t.LangId == LangIdCompose)
                .OrderBy(t => t.CreatedTime)
                .ToList();
        }

        public IActionResult OnPostDelete([FromForm] int Id)
        {
            try
            {
                var item = Db.GetOne<PP_Page>(Id);
                if (item == null)
                {
                    return new JsonResult(new { success = false, message = "Không tìm thấy trang!" });
                }

                Db.Delete<PP_Page>(item.Id);
                return new JsonResult(new { success = true, message = $"Mục [{item.Title}] đã được xóa!" });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }
    }
}

