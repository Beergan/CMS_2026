using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public async Task OnGetAsync()
        {
            Registers = (await Db.GetListAsync<PP_Register>())
                .OrderByDescending(t => t.CreatedTime)
                .ToList();
        }

        public async Task<IActionResult> OnPostDeleteAsync([FromForm] int Id)
        {
            try
            {
                var item = await Db.GetOneAsync<PP_Register>(Id);
                if (item == null)
                {
                    return new JsonResult(new { success = false, message = "Không tìm thấy đăng ký!" });
                }

                await Db.DeleteAsync<PP_Register>(item.Id);
                return new JsonResult(new { success = true, message = "Đã xóa thành công!" });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }
    }
}
