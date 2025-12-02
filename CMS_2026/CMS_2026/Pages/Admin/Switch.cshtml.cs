using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CMS_2026.Data;
using CMS_2026.Services;
using CMS_2026.Pages.Admin;
using CMS_2026.Common;

namespace CMS_2026.Pages.Admin
{
    public class SwitchModel : BaseAdminPageModel
    {
        public SwitchModel(IDataService dataService, RootService rootService, 
            Microsoft.Extensions.Caching.Memory.IMemoryCache cache, PermissionService permissionService)
            : base(dataService, rootService, cache, permissionService)
        {
        }

        public IActionResult OnGet(string? lang, string? redirect)
        {
            if (!string.IsNullOrEmpty(lang))
            {
                LangIdCompose = lang;
            }

            if (string.IsNullOrEmpty(redirect))
            {
                redirect = Constants.Admin_Url;
            }

            return Redirect(redirect);
        }
    }
}

