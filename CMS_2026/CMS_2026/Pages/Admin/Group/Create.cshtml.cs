using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CMS_2026.Data.Entities;
using CMS_2026.Services;

namespace CMS_2026.Pages.Admin.Group
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

        public IActionResult OnPost([FromForm] string Name, [FromForm] string? Description)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Name))
                {
                    return new JsonResult(new { success = false, message = "Tên nhóm không được để trống!" });
                }

                var role = new PP_Roles
                {
                    Name = Name,
                    NormalizedName = Name.ToUpper(),
                    Description = Description
                };

                Db.Insert(role);

                return new JsonResult(new { success = true, message = "Tạo nhóm thành công!", redirect = "/admin/group" });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }
    }
}

