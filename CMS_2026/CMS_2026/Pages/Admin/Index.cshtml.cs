using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CMS_2026.Data.Entities;
using CMS_2026.Services;

namespace CMS_2026.Pages.Admin
{
    public class IndexModel : BaseAdminPageModel
    {
        public List<PP_Visit> RecentVisits { get; set; } = new();
        public List<PP_Order> RecentOrders { get; set; } = new();
        public Dictionary<string, int> UrlStats { get; set; } = new();

        public IndexModel(IDataService dataService, RootService rootService, 
            Microsoft.Extensions.Caching.Memory.IMemoryCache cache, PermissionService permissionService)
            : base(dataService, rootService, cache, permissionService)
        {
        }

        public void OnGet()
        {
            RecentVisits = Db.GetList<PP_Visit>()
                .OrderByDescending(x => x.CreatedTime)
                .Take(10)
                .ToList();

            RecentOrders = Db.GetList<PP_Order>()
                .OrderByDescending(x => x.CreatedTime)
                .Take(10)
                .ToList();

            UrlStats = VisitCounterService.UrlStats
                .OrderByDescending(t => t.Value)
                .Take(20)
                .ToDictionary(t => t.Key, t => t.Value);
        }
    }
}

