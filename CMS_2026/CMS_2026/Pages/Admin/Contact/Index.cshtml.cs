using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CMS_2026.Data.Entities;
using CMS_2026.Services;

namespace CMS_2026.Pages.Admin.Contact
{
    public class IndexModel : BaseAdminPageModel
    {
        public List<PP_Contact> Contacts { get; set; } = new();

        public IndexModel(IDataService dataService, RootService rootService, 
            Microsoft.Extensions.Caching.Memory.IMemoryCache cache, PermissionService permissionService)
            : base(dataService, rootService, cache, permissionService)
        {
        }

        public void OnGet()
        {
            Contacts = Db.GetList<PP_Contact>()
                .OrderByDescending(t => t.CreatedTime)
                .ToList();
        }
    }
}

