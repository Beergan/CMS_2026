using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CMS_2026.Services;
using CMS_2026.Data;

namespace CMS_2026.Pages.Admin.CKFinder
{
    public class IndexModel : BaseAdminPageModel
    {
        public IndexModel(IDataService dataService, RootService rootService, 
            Microsoft.Extensions.Caching.Memory.IMemoryCache cache, 
            PermissionService permissionService)
            : base(dataService, rootService, cache, permissionService)
        {
        }

        public IActionResult OnGet()
        {
            // Authentication is already checked by BaseAdminPageModel
            return Page();
        }
    }
}

