using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using CMS_2026.Data.Entities;
using CMS_2026.Models;
using CMS_2026.Services;

namespace CMS_2026.Common
{
    /// <summary>
    /// Static class Root - Tương đương với Root.cs trong everflorcom_new
    /// Cho phép gọi từ bất kỳ đâu: Root.Langs, Root.Configs, Root.ShortId(), etc.
    /// </summary>
    public static class Root
    {
        // Static dictionaries - Tương đương với everflorcom_new
        public static System.Collections.Concurrent.ConcurrentDictionary<string, DateTime> CacheTable => RootService.CacheTable;
        public static System.Collections.Concurrent.ConcurrentDictionary<string, PP_Lang> Langs => RootService.Langs;
        public static System.Collections.Concurrent.ConcurrentDictionary<string, PP_Config> Configs => RootService.Configs;
        public static System.Collections.Concurrent.ConcurrentDictionary<int, int[]> CategoryIndexes => RootService.CategoryIndexes;

        /// <summary>
        /// Get RootService instance from HttpContext (DI)
        /// </summary>
        private static RootService? GetRootService()
        {
            var httpContext = GetHttpContext();
            if (httpContext == null)
                return null;

            return httpContext.RequestServices.GetService<RootService>();
        }

        /// <summary>
        /// Get HttpContext from current request
        /// </summary>
        private static HttpContext? GetHttpContext()
        {
            // Try to get from HttpContextAccessor if available
            var httpContextAccessor = GetService<IHttpContextAccessor>();
            return httpContextAccessor?.HttpContext;
        }

        /// <summary>
        /// Get service from DI container
        /// </summary>
        private static T? GetService<T>() where T : class
        {
            var httpContext = GetHttpContext();
            return httpContext?.RequestServices.GetService<T>();
        }

        /// <summary>
        /// Reload languages - Tương đương với Root.ReloadLangs() trong everflorcom_new
        /// </summary>
        public static void ReloadLangs(List<PP_Lang>? langs = null)
        {
            var rootService = GetRootService();
            rootService?.ReloadLangs(langs);
        }

        /// <summary>
        /// Refresh configs - Tương đương với Root.RefreshConfigs() trong everflorcom_new
        /// </summary>
        public static void RefreshConfigs(List<PP_Config>? configs = null)
        {
            var rootService = GetRootService();
            rootService?.RefreshConfigs(configs);
        }

        /// <summary>
        /// Refresh category indexes - Tương đương với Root.RefreshCategoryIndexes() trong everflorcom_new
        /// </summary>
        public static void RefreshCategoryIndexes()
        {
            var rootService = GetRootService();
            rootService?.RefreshCategoryIndexes();
        }

        /// <summary>
        /// Clear cache - Tương đương với Root.ClearCache() trong everflorcom_new
        /// </summary>
        public static void ClearCache()
        {
            var rootService = GetRootService();
            rootService?.ClearCache();
        }

        /// <summary>
        /// Generate short ID - Tương đương với Root.ShortId() trong everflorcom_new
        /// </summary>
        public static string ShortId()
        {
            var rootService = GetRootService();
            return rootService?.ShortId() ?? Guid.NewGuid().ToString("N")[..8];
        }

        /// <summary>
        /// Get config for language - Tương đương với Root.Config trong everflorcom_new
        /// Tự động lấy LangId từ HttpContext nếu có
        /// </summary>
        public static Config GetConfig(string? langId = null)
        {
            // Auto-detect LangId from HttpContext if not provided
            if (string.IsNullOrEmpty(langId))
            {
                var httpContext = GetHttpContext();
                if (httpContext != null)
                {
                    // Try to get from route data
                    if (httpContext.Request.RouteValues.TryGetValue("LangId", out var routeLangId))
                    {
                        langId = routeLangId?.ToString();
                    }
                    // Try to get from cookie
                    else if (httpContext.Request.Cookies.TryGetValue("LangId", out var cookieLangId))
                    {
                        langId = cookieLangId;
                    }
                    // Try to get from query string
                    else if (httpContext.Request.Query.TryGetValue("lang", out var queryLangId))
                    {
                        langId = queryLangId.ToString();
                    }
                }
            }

            langId ??= "vi";

            var rootService = GetRootService();
            return rootService?.GetConfig(langId) ?? new Config();
        }

        /// <summary>
        /// Config property - Tương đương với Root.Config trong everflorcom_new
        /// Tự động lấy LangId từ HttpContext
        /// </summary>
        public static Config Config => GetConfig();

        /// <summary>
        /// Current website URL - Tương đương với Root.CurrentWebsiteUrl trong everflorcom_new
        /// </summary>
        public static string CurrentWebsiteUrl
        {
            get
            {
                var rootService = GetRootService();
                return rootService?.CurrentWebsiteUrl ?? string.Empty;
            }
        }

        /// <summary>
        /// Layout ID - Tương đương với Root.LayoutId trong everflorcom_new
        /// </summary>
        public static string LayoutId
        {
            get
            {
                var rootService = GetRootService();
                return rootService?.LayoutId ?? string.Empty;
            }
        }

        /// <summary>
        /// Decode JSON - Tương đương với Root.DecodeJson<T>() trong everflorcom_new
        /// </summary>
        public static T? DecodeJson<T>(string? json)
        {
            return RootService.DecodeJson<T>(json);
        }

        /// <summary>
        /// Is true value - Tương đương với Root.IsTrueValue() trong everflorcom_new
        /// </summary>
        public static bool IsTrueValue(object? obj)
        {
            return RootService.IsTrueValue(obj);
        }

        /// <summary>
        /// Get route data - Tương đương với Root.GetRouteData<T>() trong everflorcom_new
        /// </summary>
        public static T? GetRouteData<T>(string key)
        {
            var httpContext = GetHttpContext();
            if (httpContext == null)
                return default(T);

            if (httpContext.Request.RouteValues.TryGetValue(key, out var value))
            {
                if (value is T tValue)
                    return tValue;
                
                try
                {
                    return (T)Convert.ChangeType(value, typeof(T));
                }
                catch
                {
                    return default(T);
                }
            }

            return default(T);
        }

        /// <summary>
        /// Get context data - Tương đương với Root.GetContextData<T>() trong everflorcom_new
        /// </summary>
        public static T? GetContextData<T>(string key, Func<T>? setup = null) where T : class
        {
            var httpContext = GetHttpContext();
            if (httpContext == null)
                return setup?.Invoke();

            if (httpContext.Items.TryGetValue(key, out var value) && value is T tValue)
            {
                return tValue;
            }

            if (setup != null)
            {
                var newValue = setup();
                httpContext.Items[key] = newValue;
                return newValue;
            }

            return default(T);
        }

        /// <summary>
        /// Get cookie data - Tương đương với Root.GetCookieData() trong everflorcom_new
        /// </summary>
        public static string GetCookieData(string key, Func<string>? setup = null)
        {
            var httpContext = GetHttpContext();
            if (httpContext == null)
                return setup?.Invoke() ?? string.Empty;

            if (httpContext.Request.Cookies.TryGetValue(key, out var value) && !string.IsNullOrEmpty(value))
            {
                return value;
            }

            if (setup != null)
            {
                var newValue = setup();
                httpContext.Response.Cookies.Append(key, newValue, new Microsoft.AspNetCore.Http.CookieOptions
                {
                    Expires = DateTime.Now.AddYears(1)
                });
                return newValue;
            }

            return string.Empty;
        }
    }
}

