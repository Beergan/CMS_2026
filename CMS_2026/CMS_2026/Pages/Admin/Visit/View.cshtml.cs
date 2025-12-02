using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CMS_2026.Data.Entities;
using CMS_2026.Services;

namespace CMS_2026.Pages.Admin.Visit
{
    public class ViewModel : BaseAdminPageModel
    {
        public PP_Visit? Visit { get; set; }

        public ViewModel(IDataService dataService, RootService rootService, 
            Microsoft.Extensions.Caching.Memory.IMemoryCache cache, PermissionService permissionService)
            : base(dataService, rootService, cache, permissionService)
        {
        }

        public IActionResult OnGet(int? id)
        {
            if (!id.HasValue)
            {
                return Redirect("/admin/visit");
            }

            Visit = Db.GetOne<PP_Visit>(id.Value);
            if (Visit == null)
            {
                return Redirect("/admin/visit");
            }

            return Page();
        }
    }
}

