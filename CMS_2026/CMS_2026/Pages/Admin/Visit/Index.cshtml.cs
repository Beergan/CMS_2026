using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CMS_2026.Data.Entities;
using CMS_2026.Services;

namespace CMS_2026.Pages.Admin.Visit
{
    public class IndexModel : BaseAdminPageModel
    {
        public List<PP_Visit> Visits { get; set; } = new();

        public IndexModel(IDataService dataService, RootService rootService, 
            Microsoft.Extensions.Caching.Memory.IMemoryCache cache, PermissionService permissionService)
            : base(dataService, rootService, cache, permissionService)
        {
        }

        public async Task OnGetAsync()
        {
            var visits = await Db.GetListAsync<PP_Visit>();
            Visits = visits
                .OrderByDescending(t => t.CreatedTime)
                .Take(100)
                .ToList();
        }
    }
}

