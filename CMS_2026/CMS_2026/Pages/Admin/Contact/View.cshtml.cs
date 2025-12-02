using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CMS_2026.Data.Entities;
using CMS_2026.Services;

namespace CMS_2026.Pages.Admin.Contact
{
    public class ViewModel : BaseAdminPageModel
    {
        public PP_Contact? Contact { get; set; }

        public ViewModel(IDataService dataService, RootService rootService, 
            Microsoft.Extensions.Caching.Memory.IMemoryCache cache, PermissionService permissionService)
            : base(dataService, rootService, cache, permissionService)
        {
        }

        public IActionResult OnGet(int? id)
        {
            if (!id.HasValue)
            {
                return Redirect("/admin/contact");
            }

            Contact = Db.GetOne<PP_Contact>(id.Value);
            if (Contact == null)
            {
                return Redirect("/admin/contact");
            }

            return Page();
        }
    }
}

