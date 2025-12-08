using CMS_2026.Common;
using CMS_2026.Data.Entities;
using CMS_2026.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Reflection;
using System.Text;

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
            if (cache.TryGetValue(key, out object? cachedObj))
            {
                if (cachedObj != null)
                {
                    var cachedType = cachedObj.GetType();
                    var dataProperty = cachedType.GetProperty("Data");
                    if (dataProperty != null)
                    {
                        var dataValue = dataProperty.GetValue(cachedObj);
                        if (dataValue is T cachedData)
                        {
                            return cachedData;
                        }
                    }
                    // If it's already T (new format), return directly
                    else if (cachedObj is T directCached)
                    {
                        return directCached;
                    }
                }
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
                    System.Diagnostics.Debug.WriteLine($"LoadData deserialization error: {ex.Message}");
                }
            }

            if (setup != null)
            {
                data = setup(data);
            }

            if (data != null)
            {
                using (var entry = cache.CreateEntry(key))
                {
                    // Store data directly (not wrapped) to match TryGetValue<T> behavior
                    entry.Value = data;
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30);
                    
                    // Track cache entry in CacheTable for IncrementCacheVersion to work
                    RootService.CacheTable.TryAdd(key, DateTime.Now);
                }
            }
            //var json = JsonConvert.SerializeObject(data);
            //long bytes = Encoding.UTF8.GetByteCount(json);
            //double mb = bytes / 1024.0 / 1024.0;
            //Console.WriteLine($"REAL Cache Size ≈ {mb} MB");
            return data;
        }
    }
}

