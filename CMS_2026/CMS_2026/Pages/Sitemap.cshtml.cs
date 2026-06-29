using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CMS_2026.Data;
using CMS_2026.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Memory;

namespace CMS_2026.Pages
{
    public class SitemapModel : PageModel
    {
        private readonly IDataService _dataService;
        private readonly RootService _rootService;
        private readonly IMemoryCache _cache;

        private const string CacheKey = "sitemap_xml_urls";
        private static readonly TimeSpan CacheDuration = TimeSpan.FromHours(1);

        public SitemapModel(IDataService dataService, RootService rootService, IMemoryCache cache)
        {
            _dataService = dataService;
            _rootService = rootService;
            _cache = cache;
        }

        public List<SitemapUrl> Urls { get; set; } = new();

        public async Task OnGetAsync()
        {
            // Trả cache ngay nếu có — tránh query DB mỗi lần Google bot crawl
            if (_cache.TryGetValue(CacheKey, out List<SitemapUrl>? cached) && cached != null)
            {
                Urls = cached;
                return;
            }

            var result = new List<SitemapUrl>();
            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            var now = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss+00:00");

            // Homepage
            result.Add(new SitemapUrl
            {
                Location = baseUrl,
                LastMod = now,
                Priority = "1.00"
            });

            // Get all languages
            var languages = await _dataService.GetListAsync<CMS_2026.Data.Entities.PP_Lang>(l => l.Status == "ACTIVE");

            foreach (var lang in languages)
            {
                var config = _rootService.GetConfig(lang.LangId);

                // Pages
                var pages = await _dataService.GetListAsync<CMS_2026.Data.Entities.PP_Page>(
                    p => p.LangId == lang.LangId && p.Status == "ACTIVE");

                foreach (var page in pages)
                {
                    result.Add(new SitemapUrl
                    {
                        Location = $"{baseUrl}/{lang.LangId}/{page.NodePath}",
                        LastMod = page.UpdatedTime?.ToString("yyyy-MM-ddTHH:mm:ss+00:00") ?? now,
                        Priority = "0.80"
                    });
                }

                // Products
                var products = await _dataService.GetListAsync<CMS_2026.Data.Entities.PP_Product>(
                    p => p.LangId == lang.LangId && p.Status == "ACTIVE");

                foreach (var product in products)
                {
                    result.Add(new SitemapUrl
                    {
                        Location = $"{baseUrl}/{lang.LangId}/san-pham/{product.NodePath}",
                        LastMod = product.UpdatedTime?.ToString("yyyy-MM-ddTHH:mm:ss+00:00") ?? now,
                        Priority = "0.80"
                    });
                }

                // Posts/Blogs
                var posts = await _dataService.GetListAsync<CMS_2026.Data.Entities.PP_Node>(
                    p => p.LangId == lang.LangId && p.NodeType == "post" && p.Status == "ACTIVE");

                foreach (var post in posts)
                {
                    result.Add(new SitemapUrl
                    {
                        Location = $"{baseUrl}/{lang.LangId}/tin-tuc/{post.NodePath}",
                        LastMod = post.UpdatedTime?.ToString("yyyy-MM-ddTHH:mm:ss+00:00") ?? now,
                        Priority = "0.80"
                    });
                }
            }

            // Lưu cache 1 giờ — tự invalidate khi admin gọi RootService.ClearCache()
            _cache.Set(CacheKey, result, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = CacheDuration,
                Priority = CacheItemPriority.Normal
            });
            RootService.CacheTable.TryAdd(CacheKey, DateTime.Now);

            Urls = result;
        }
    }

    public class SitemapUrl
    {
        public string Location { get; set; } = string.Empty;
        public string LastMod { get; set; } = string.Empty;
        public string Priority { get; set; } = "0.50";
    }
}

