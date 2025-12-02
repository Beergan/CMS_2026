using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CMS_2026.Data.Entities;
using CMS_2026.Services;
using ConfigModel = CMS_2026.Models.Config;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CMS_2026.Pages.Admin.Config
{
    public static class JsonHelper
    {
        public static Dictionary<string, object> DeserializeToDictionary(string json)
        {
            if (string.IsNullOrEmpty(json))
                return new Dictionary<string, object>();

            try
            {
                var obj = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
                if (obj == null)
                    return new Dictionary<string, object>();

                // Convert JObject và JArray thành Dictionary và List
                return ConvertJTokenToDictionary(obj);
            }
            catch
            {
                return new Dictionary<string, object>();
            }
        }

        private static Dictionary<string, object> ConvertJTokenToDictionary(Dictionary<string, object> dict)
        {
            var result = new Dictionary<string, object>();
            foreach (var kvp in dict)
            {
                result[kvp.Key] = ConvertJToken(kvp.Value);
            }
            return result;
        }

        private static object ConvertJToken(object? value)
        {
            if (value == null)
                return value!;

            if (value is JObject jObject)
            {
                var dict = new Dictionary<string, object>();
                foreach (var prop in jObject.Properties())
                {
                    dict[prop.Name] = ConvertJToken(prop.Value);
                }
                return dict;
            }
            else if (value is JArray jArray)
            {
                var list = new List<object>();
                foreach (var item in jArray)
                {
                    list.Add(ConvertJToken(item));
                }
                return list;
            }
            else if (value is JValue jValue)
            {
                return jValue.Value ?? "";
            }
            else if (value is JToken)
            {
                // Fallback: convert to string then deserialize
                try
                {
                    var jsonString = value.ToString();
                    var tempObj = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonString ?? "{}");
                    if (tempObj != null)
                        return ConvertJTokenToDictionary(tempObj);
                }
                catch { }
            }

            return value;
        }
    }

    public class IndexModel : BaseAdminPageModel
    {
        public string? ConfigJson { get; set; }
        public PP_Config? RootConfig { get; set; }
        public List<PP_Compt> LayoutCompts { get; set; } = new();
        public List<PP_Config> LayoutConfigs { get; set; } = new();
        public List<System.Tuple<string, string>> LinkOptions { get; set; } = new();

        public IndexModel(IDataService dataService, RootService rootService, 
            Microsoft.Extensions.Caching.Memory.IMemoryCache cache, PermissionService permissionService)
            : base(dataService, rootService, cache, permissionService)
        {
        }

        public void OnGet()
        {
            RootConfig = Db.GetOne<PP_Config>(t => t.LangId == LangIdCompose && t.ConfigKey == "root");
            if (RootConfig != null && !string.IsNullOrEmpty(RootConfig.JsonContent))
            {
                ConfigJson = RootConfig.JsonContent;
            }
            else
            {
                var defaultConfig = new ConfigModel();
                ConfigJson = JsonConvert.SerializeObject(defaultConfig);
            }

            // Query layout components: có thể dựa trên PageType = "layout" hoặc ComptKey LIKE 'layout_%'
            var layoutIdPrefix = string.IsNullOrEmpty(Root.LayoutId) ? "layout_" : $"layout{Root.LayoutId}_";
            LayoutCompts = Db.GetList<PP_Compt>(t => 
                (t.PageType != null && t.PageType == "layout") || 
                (t.ComptKey != null && t.ComptKey.StartsWith(layoutIdPrefix)));
            LayoutConfigs = Db.GetList<PP_Config>(t => t.LangId == LangIdCompose && t.PageId == 0);
            LinkOptions = Db.GetLinks(LangIdCompose);
        }

        public IActionResult OnPostUpdateConfig([FromForm] string JsonData)
        {
            try
            {
                // Validate JSON
                var newData = JsonConvert.DeserializeObject<Dictionary<string, object>>(JsonData);
                if (newData == null)
                {
                    return new JsonResult(new { success = false, message = "Dữ liệu JSON không hợp lệ!" });
                }

                var rootConfig = Db.GetOne<PP_Config>(t => t.LangId == LangIdCompose && t.ConfigKey == "root");
                
                if (rootConfig != null && !string.IsNullOrEmpty(rootConfig.JsonContent))
                {
                    // Merge với dữ liệu cũ để giữ lại các fields không có trong form
                    var oldData = JsonHelper.DeserializeToDictionary(rootConfig.JsonContent);
                    
                    // Merge: dữ liệu mới ghi đè dữ liệu cũ, nhưng giữ lại các keys không có trong dữ liệu mới
                    foreach (var kvp in oldData)
                    {
                        if (!newData.ContainsKey(kvp.Key))
                        {
                            newData[kvp.Key] = kvp.Value;
                        }
                    }
                    
                    rootConfig.JsonContent = JsonConvert.SerializeObject(newData);
                    Db.Update(rootConfig);
                }
                else
                {
                    rootConfig = new PP_Config
                    {
                        LangId = LangIdCompose,
                        ConfigKey = "root",
                        JsonContent = JsonConvert.SerializeObject(newData)
                    };
                    Db.Insert(rootConfig);
                }

                Root.RefreshConfigs();

                return new JsonResult(new { success = true, message = "Cập nhật thành công!" });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }

        public IActionResult OnPostUpdateCompt([FromForm] string? action, [FromForm] string? langId, 
            [FromForm] string? comptKey, [FromForm] string? jsonData)
        {
            try
            {
                // Validate JSON
                JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonData ?? "{}");

                // Tìm config: có thể là ComptKey hoặc layout_ComptKey
                var comptConfig = Db.GetOne<PP_Config>(t => t.LangId == langId
                    && t.PageId == 0
                    && (t.ConfigKey == comptKey || t.ConfigKey == $"layout_{comptKey}"));

                // Nếu không tìm thấy, thử tìm theo ComptKey trong bảng Compt
                if (comptConfig == null && !string.IsNullOrEmpty(comptKey))
                {
                    var component = Db.GetOne<PP_Compt>(t => t.ComptKey == comptKey);
                    if (component != null && component.PageType == "layout")
                    {
                        // Với layout components, có thể cần prefix layout_
                        var layoutIdPrefix = string.IsNullOrEmpty(Root.LayoutId) ? "layout_" : $"layout{Root.LayoutId}_";
                        var possibleConfigKey = $"{layoutIdPrefix}{comptKey}";
                        comptConfig = Db.GetOne<PP_Config>(t => t.LangId == langId
                            && t.PageId == 0
                            && t.ConfigKey == possibleConfigKey);
                        
                        // Nếu vẫn không tìm thấy, dùng ComptKey trực tiếp
                        if (comptConfig == null)
                        {
                            comptConfig = Db.GetOne<PP_Config>(t => t.LangId == langId
                                && t.PageId == 0
                                && t.ConfigKey == comptKey);
                        }
                    }
                }

                if (comptConfig != null)
                {
                    if (action == "reset")
                    {
                        var component = Db.GetOne<PP_Compt>(t => t.ComptKey == comptKey);
                        if (component != null)
                        {
                            comptConfig.JsonContent = component.JsonDefault;
                            Db.Update(comptConfig);
                        }
                    }
                    else
                    {
                        // Merge với dữ liệu cũ để giữ lại các fields không có trong form
                        var newData = JsonHelper.DeserializeToDictionary(jsonData ?? "{}");
                        if (!string.IsNullOrEmpty(comptConfig.JsonContent))
                        {
                            var oldData = JsonHelper.DeserializeToDictionary(comptConfig.JsonContent);
                            
                            // Merge: dữ liệu mới ghi đè dữ liệu cũ, nhưng giữ lại các keys không có trong dữ liệu mới
                            foreach (var kvp in oldData)
                            {
                                if (!newData.ContainsKey(kvp.Key))
                                {
                                    newData[kvp.Key] = kvp.Value;
                                }
                            }
                        }
                        
                        comptConfig.JsonContent = JsonConvert.SerializeObject(newData);
                        Db.Update(comptConfig);
                    }

                    Root.RefreshConfigs();
                }
                else
                {
                    // Xác định ConfigKey: với layout components có thể cần prefix
                    string configKey = comptKey ?? "";
                    if (!string.IsNullOrEmpty(comptKey))
                    {
                        var component = Db.GetOne<PP_Compt>(t => t.ComptKey == comptKey);
                        if (component != null && component.PageType == "layout")
                        {
                            // Kiểm tra xem trong DB có config nào với layout_ prefix không
                            var layoutIdPrefix = string.IsNullOrEmpty(Root.LayoutId) ? "layout_" : $"layout{Root.LayoutId}_";
                            var existingWithPrefix = Db.GetOne<PP_Config>(t => t.LangId == (langId ?? LangIdCompose)
                                && t.PageId == 0
                                && t.ConfigKey.StartsWith($"{layoutIdPrefix}{comptKey}"));
                            
                            if (existingWithPrefix != null)
                            {
                                configKey = existingWithPrefix.ConfigKey;
                            }
                            else
                            {
                                // Dùng ComptKey trực tiếp (giống code cũ)
                                configKey = comptKey;
                            }
                        }
                        else
                        {
                            configKey = comptKey;
                        }
                    }

                    comptConfig = new PP_Config
                    {
                        LangId = langId ?? LangIdCompose,
                        PageId = 0,
                        ConfigKey = configKey,
                        JsonContent = jsonData
                    };

                    Db.Insert(comptConfig);
                    Root.RefreshConfigs();
                }

                return new JsonResult(new { success = true, message = "Cập nhật thành công!" });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }
    }
}

