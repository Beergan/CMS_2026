using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using CMS_2026.Services;
using CMS_2026.Common;
using Newtonsoft.Json;

namespace CMS_2026.Utils
{
    /// <summary>
    /// Helper class for Razor views to access LoadData functionality
    /// </summary>
    public static class ViewHelper
    {
        /// <summary>
        /// Load data from config (compatible with BasePageModel.LoadData)
        /// </summary>
        public static T? LoadData<T>(
            HttpContext context,
            RootService root,
            IMemoryCache cache,
            string key,
            Func<T?, T?>? setup = null) where T : class
        {
            // Try to get from cache first (20 minutes expiration)
            if (cache.TryGetValue(key, out T? cached))
            {
                return cached;
            }

            // Get LangId and PageId from context
            var langId = context.Request.Cookies["LangId"] ?? "vi";
            var pageId = context.Items.TryGetValue("PageId", out var pageIdObj) && pageIdObj is int id ? id : 0;
            
            // Get FileName from context
            string fileName;
            if (context.Items.TryGetValue("ComptKey", out var comptKey) && comptKey is string keyValue && !string.IsNullOrEmpty(keyValue))
            {
                fileName = keyValue;
            }
            else
            {
                var path = context.Request.Path.Value ?? string.Empty;
                fileName = path.Split('/').LastOrDefault()?.Split('.').FirstOrDefault() ?? string.Empty;
            }

            // Try to load from config (same logic as BasePageModel)
            // Priority: PageId-specific config > Root config (PageId = 0)
            T? data = null;
            var config = Root.Configs.Values
                .FirstOrDefault(t => t.LangId == langId && 
                                    (t.PageId == pageId || t.PageId == 0) && 
                                    t.ConfigKey == fileName);

            if (config != null && !string.IsNullOrEmpty(config.JsonContent))
            {
                try
                {
                    data = JsonConvert.DeserializeObject<T>(config.JsonContent);
                }
                catch (Exception ex)
                {
                    // Log error but don't throw
                    System.Diagnostics.Debug.WriteLine($"LoadData deserialization error: {ex.Message}");
                }
            }

            // Run setup function if provided
            if (setup != null)
            {
                data = setup(data);
            }

            // Cache the result (20 minutes)
            if (data != null)
            {
                using (var entry = cache.CreateEntry(key))
                {
                    entry.Value = data;
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(20);
                }
            }

            return data;
        }
    }
}

