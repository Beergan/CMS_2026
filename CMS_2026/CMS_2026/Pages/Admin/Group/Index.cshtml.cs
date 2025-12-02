using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CMS_2026.Data.Entities;
using CMS_2026.Services;

namespace CMS_2026.Pages.Admin.Group
{
    public class IndexModel : BaseAdminPageModel
    {
        public List<PP_Roles> Groups { get; set; } = new();

        public IndexModel(IDataService dataService, RootService rootService, 
            Microsoft.Extensions.Caching.Memory.IMemoryCache cache, PermissionService permissionService)
            : base(dataService, rootService, cache, permissionService)
        {
        }

        public void OnGet()
        {
            Groups = Db.GetList<PP_Roles>()
                .OrderBy(t => t.RoleName)
                .ToList();
        }
    }
}

