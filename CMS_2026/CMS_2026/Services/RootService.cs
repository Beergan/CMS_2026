using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using CMS_2026.Data.Entities;
using CMS_2026.Models;
using CMS_2026.Services;
using Newtonsoft.Json;

namespace CMS_2026.Services
{
    public class RootService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IMemoryCache _cache;
        private readonly IConfiguration _configuration;

        public static ConcurrentDictionary<string, DateTime> CacheTable = new();
        public static ConcurrentDictionary<string, PP_Lang> Langs = new();
        public static ConcurrentDictionary<string, PP_Config> Configs = new();
        public static ConcurrentDictionary<int, int[]> CategoryIndexes = new();

        private static Dictionary<string, Config> _rootConfig = new();

        public RootService(IServiceScopeFactory serviceScopeFactory, IMemoryCache cache, IConfiguration configuration)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _cache = cache;
            _configuration = configuration;
        }

        public void ReloadLangs(List<PP_Lang>? langs = null)
        {
            if (langs == null)
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var dataService = scope.ServiceProvider.GetRequiredService<IDataService>();
                langs = dataService.GetList<PP_Lang>();
            }
            
            foreach (var lang in langs)
            {
                Langs.AddOrUpdate(lang.LangId, lang, (key, oldValue) => lang);
            }
        }

        public void RefreshConfigs(List<PP_Config>? configs = null)
        {
            if (configs == null)
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var dataService = scope.ServiceProvider.GetRequiredService<IDataService>();
                configs = dataService.GetList<PP_Config>();
            }
            
            foreach (var config in configs)
            {
                var key = $"{config.LangId}-{config.PageId}-{config.ConfigKey}";
                Configs.AddOrUpdate(key, config, (k, oldValue) => config);
            }

            // Clear cache
            foreach (var key in CacheTable.Keys)
            {
                _cache.Remove(key);
            }

            _rootConfig.Clear();
        }

        public Config GetConfig(string langId = "vi")
        {
            if (_rootConfig == null || !_rootConfig.ContainsKey(langId))
            {
                _rootConfig ??= new Dictionary<string, Config>();
                
                var configKey = $"{langId}-0-root";
                if (Configs.TryGetValue(configKey, out var config) && !string.IsNullOrEmpty(config.JsonContent))
                {
                    _rootConfig[langId] = JsonConvert.DeserializeObject<Config>(config.JsonContent) ?? new Config();
                }
                else
                {
                    _rootConfig[langId] = new Config();
                }
            }

            return _rootConfig[langId];
        }

        public void RefreshCategoryIndexes()
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var dataService = scope.ServiceProvider.GetRequiredService<IDataService>();
            var indexes = dataService.GetCategoryIndexes();
            foreach (var index in indexes)
            {
                var array = index.Array.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(int.Parse)
                    .ToArray();
                CategoryIndexes.AddOrUpdate(index.RootId, array, (key, oldValue) => array);
            }
        }

        public void ClearCache()
        {
            foreach (var key in CacheTable.Keys)
            {
                _cache.Remove(key);
                CacheTable.TryRemove(key, out _);
            }
        }

        public string ShortId()
        {
            // Implementation for short ID generation
            return Guid.NewGuid().ToString("N")[..8];
        }

        public string CurrentWebsiteUrl => _configuration["Website"] ?? string.Empty;
        public string LayoutId => _configuration["LayoutId"] ?? string.Empty;

        public static T? DecodeJson<T>(string? json)
        {
            if (string.IsNullOrEmpty(json))
                return default(T);

            try
            {
                return JsonConvert.DeserializeObject<T>(json);
            }
            catch
            {
                return default(T);
            }
        }

        public static bool IsTrueValue(object? obj)
        {
            if (obj == null) return false;
            if (obj is bool boolValue) return boolValue;
            if (obj is string strValue)
            {
                return strValue.Equals("true", StringComparison.OrdinalIgnoreCase) ||
                       strValue.Equals("True", StringComparison.OrdinalIgnoreCase) ||
                       strValue == "1";
            }
            return false;
        }
    }
}

