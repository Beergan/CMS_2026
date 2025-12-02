using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CMS_2026.Data.Entities;
using CMS_2026.Services;

namespace CMS_2026.Pages.Admin.Register
{
    public class IndexModel : BaseAdminPageModel
    {
        public List<PP_Register> Registers { get; set; } = new();

        public IndexModel(IDataService dataService, RootService rootService, 
            Microsoft.Extensions.Caching.Memory.IMemoryCache cache, PermissionService permissionService)
            : base(dataService, rootService, cache, permissionService)
        {
        }

        public void OnGet()
        {
            Registers = Db.GetList<PP_Register>()
                .OrderByDescending(t => t.CreatedTime)
                .ToList();
        }

        public IActionResult OnPostDelete([FromForm] int Id)
        {
            try
            {
                var item = Db.GetOne<PP_Register>(Id);
                if (item == null)
                {
                    return new JsonResult(new { success = false, message = "Không tìm thấy đăng ký!" });
                }

                Db.Delete<PP_Register>(item.Id);
                return new JsonResult(new { success = true, message = "Đã xóa thành công!" });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }
    }
}

