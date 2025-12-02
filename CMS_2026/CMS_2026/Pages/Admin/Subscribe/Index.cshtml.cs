using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CMS_2026.Data.Entities;
using CMS_2026.Services;

namespace CMS_2026.Pages.Admin.Subscribe
{
    public class IndexModel : BaseAdminPageModel
    {
        public List<PP_Subscribe> Subscribes { get; set; } = new();

        public IndexModel(IDataService dataService, RootService rootService, 
            Microsoft.Extensions.Caching.Memory.IMemoryCache cache, PermissionService permissionService)
            : base(dataService, rootService, cache, permissionService)
        {
        }

        public void OnGet()
        {
            Subscribes = Db.GetList<PP_Subscribe>()
                .OrderByDescending(t => t.CreatedTime)
                .ToList();
        }
    }
}

