using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CMS_2026.Data.Entities;
using CMS_2026.Services;

namespace CMS_2026.Pages
{
    public class DynamicPageModel : BasePageModel
    {
        private readonly PageRoutingService _pageRoutingService;

        public string? ComponentPath { get; set; }
        public PP_Page? PageData { get; set; }

        public DynamicPageModel(IDataService dataService, RootService rootService, 
            Microsoft.Extensions.Caching.Memory.IMemoryCache cache, PageRoutingService pageRoutingService)
            : base(dataService, rootService, cache)
        {
            _pageRoutingService = pageRoutingService;
        }

        public IActionResult OnGet()
        {
            // Get page data from HttpContext (set by DynamicPageMiddleware)
            // This is more efficient than re-querying (compatible with 4.8 approach)
            if (!HttpContext.Items.TryGetValue("PP_Page", out var pageObj) || 
                pageObj is not PP_Page page)
            {
                return NotFound();
            }

            PageData = page;
            ComponentPath = _pageRoutingService.GetComponentPath(page);

            if (ComponentPath == null)
            {
                return NotFound();
            }

            // Page data already stored in HttpContext by middleware
            // This ensures components can access PageId, LangId, ComptKey, NodeSlug

            return Page();
        }
    }
}

