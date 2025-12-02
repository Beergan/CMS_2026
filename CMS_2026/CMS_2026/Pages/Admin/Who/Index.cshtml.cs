using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CMS_2026.Data.Entities;
using CMS_2026.Services;

namespace CMS_2026.Pages.Admin.Who
{
    public class IndexModel : BaseAdminPageModel
    {
        public List<PP_Visit> OnlineSessions { get; set; } = new();
        public int OnlineCount => OnlineSessions.Count;

        public IndexModel(IDataService dataService, RootService rootService, 
            Microsoft.Extensions.Caching.Memory.IMemoryCache cache, PermissionService permissionService)
            : base(dataService, rootService, cache, permissionService)
        {
        }

        public void OnGet()
        {
            OnlineSessions = VisitCounterService.OnlineSessions.Values.ToList();
        }
    }
}

