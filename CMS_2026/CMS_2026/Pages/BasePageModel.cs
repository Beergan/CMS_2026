using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Memory;
using CMS_2026.Data.Entities;
using CMS_2026.Models;
using CMS_2026.Services;
using CMS_2026.Common;
using Newtonsoft.Json;

namespace CMS_2026.Pages
{
    public class BasePageModel : PageModel
    {
        protected readonly IDataService Db;
        protected readonly RootService Root;
        protected readonly IMemoryCache Cache;

        public BasePageModel(IDataService dataService, RootService rootService, IMemoryCache cache)
        {
            Db = dataService;
            Root = rootService;
            Cache = cache;
        }

        protected string LangId
        {
            get
            {
                if (HttpContext.Items.TryGetValue("LangId", out var langId))
                {
                    return langId?.ToString() ?? "vi";
                }
                return Request.Query["lang"].FirstOrDefault() ?? "vi";
            }
        }

        protected int PageId
        {
            get
            {
                if (HttpContext.Items.TryGetValue("PageId", out var pageId) && pageId is int id)
                {
                    return id;
                }
                return 0;
            }
        }

        /// <summary>
        /// Get node slug from route (compatible with 4.8 NodeSlug)
        /// In 4.8: NodeSlug => HttpContext.Current.Request.Path.GetAfter("/").GetBefore("?")
        /// In Core: We get from HttpContext.Items (set by middleware) or extract from path
        /// </summary>
        protected string? NodeSlug
        {
            get
            {
                // Try to get from HttpContext (set by middleware for dynamic routes)
                if (HttpContext.Items.TryGetValue("NodeSlug", out var slug) && 
                    slug is string slugValue && !string.IsNullOrEmpty(slugValue))
                {
                    return slugValue;
                }

                // Fallback: extract from path (similar to 4.8 logic)
                var path = Request.Path.Value ?? string.Empty;
                return path.TrimStart('/').Split('?').FirstOrDefault();
            }
        }

        protected int CurrentPage
        {
            get
            {
                if (int.TryParse(Request.Query["Page"].FirstOrDefault(), out var page))
                {
                    return page;
                }
                return 1;
            }
        }

        /// <summary>
        /// Get file name from component key (compatible with 4.8 FileName logic)
        /// In 4.8: FileName => VirtualPath.GetAfterLast("/").GetBefore(".")
        /// In Core: We use ComptKey from page data
        /// </summary>
        protected string FileName
        {
            get
            {
                // Try to get from HttpContext (set by middleware)
                if (HttpContext.Items.TryGetValue("ComptKey", out var comptKey) && 
                    comptKey is string key && !string.IsNullOrEmpty(key))
                {
                    return key;
                }

                // Fallback: extract from path (similar to 4.8 logic)
                var path = Request.Path.Value ?? string.Empty;
                var fileName = path.Split('/').LastOrDefault()?.Split('.').FirstOrDefault();
                return fileName ?? string.Empty;
            }
        }

        /// <summary>
        /// Check if this is a direct call (compatible with 4.8 IsDirectCall)
        /// In 4.8: IsDirectCall => VirtualPath.GetAfter("~/") == NodeSlug
        /// </summary>
        protected bool IsDirectCall
        {
            get
            {
                // In ASP.NET Core, we consider it a direct call if we have page data
                return HttpContext.Items.ContainsKey("PP_Page");
            }
        }

        /// <summary>
        /// Check if this is a partial call (compatible with 4.8 IsPartialCall)
        /// In 4.8: IsPartialCall => VirtualPath != Request.AppRelativeCurrentExecutionFilePath
        /// </summary>
        protected bool IsPartialCall
        {
            get
            {
                // In ASP.NET Core Razor Pages, partial calls are handled via PartialAsync
                // This is mainly for compatibility
                return false;
            }
        }

        protected Config Config => CMS_2026.Common.Root.Config; // Tự động lấy LangId từ HttpContext

        /// <summary>
        /// Frontend translator - Tương đương với MyTranslator trong everflorcom_new
        /// </summary>
        protected Services.FrontendTranslator Text => new Services.FrontendTranslator(LangId);

        /// <summary>
        /// Load data from config (compatible with 4.8 LoadData logic)
        /// In 4.8: LoadData loads from pp_config table using LangId, PageId, and ConfigKey
        /// </summary>
        protected T? LoadData<T>(string key, Func<T?, T?>? setup = null) where T : class
        {
            // Try to get from cache first (20 minutes expiration like 4.8)
            if (Cache.TryGetValue(key, out T? cached))
            {
                return cached;
            }

            // Try to load from config (same logic as 4.8)
            // Priority: PageId-specific config > Root config (PageId = 0)
            T? data = null;
            var config = CMS_2026.Common.Root.Configs.Values
                .FirstOrDefault(t => t.LangId == LangId && 
                                    (t.PageId == PageId || t.PageId == 0) && 
                                    t.ConfigKey == FileName);

            if (config != null && !string.IsNullOrEmpty(config.JsonContent))
            {
                try
                {
                    data = JsonConvert.DeserializeObject<T>(config.JsonContent);
                }
                catch (Exception ex)
                {
                    // Log error but don't throw (compatible with 4.8 behavior)
                    // In 4.8, it would return null or call Response.End()
                    System.Diagnostics.Debug.WriteLine($"LoadData deserialization error: {ex.Message}");
                }
            }

            // Run setup function if provided (same as 4.8)
            if (setup != null)
            {
                data = setup(data);
            }

            // Cache the result (20 minutes like 4.8)
            if (data != null)
            {
                using (var entry = Cache.CreateEntry(key))
                {
                    entry.Value = data;
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(20);
                }
            }

            // In 4.8, if data is null and not in debug mode, it would return 404
            // In Core, we return null and let the page handle it
            return data;
        }
    }
}

