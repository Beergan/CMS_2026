using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Memory;
using CMS_2026.Services;
using CMS_2026.Pages.Admin;

namespace CMS_2026.Pages.Admin.Cache
{
    public class IndexModel : BaseAdminPageModel
    {
        private readonly RootService _rootService;
        private readonly IMemoryCache _memoryCache;

        public IndexModel(IDataService dataService, RootService rootService, IMemoryCache memoryCache, PermissionService permissionService)
            : base(dataService, rootService, memoryCache, permissionService)
        {
            _rootService = rootService;
            _memoryCache = memoryCache;
        }

        public Dictionary<string, DateTime> CacheEntries { get; set; } = new();

        public void OnGet()
        {
            // Get cache entries from RootService.CacheTable
            CacheEntries = RootService.CacheTable.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }

        public IActionResult OnPostDelete([FromForm] string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return new JsonResult(new { success = false, message = "Key không được để trống." });
            }

            try
            {
                // Remove from memory cache
                _memoryCache.Remove(key);
                
                // Remove from RootService cache table
                RootService.CacheTable.TryRemove(key, out _);

                return new JsonResult(new { success = true, message = $"Mục [{key}] đã được xóa!" });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }
    }
}

