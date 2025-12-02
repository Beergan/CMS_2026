using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CMS_2026.Data.Entities;
using CMS_2026.Services;
using CMS_2026.Utils;
using Newtonsoft.Json;

namespace CMS_2026.Pages.Admin.Page
{
    public class EditModel : BaseAdminPageModel
    {
        public new PP_Page? Page { get; set; }
        public Dictionary<string, string> PageTemplates { get; set; } = new();
        public List<PP_Compt> Components { get; set; } = new();
        public List<PP_Config> Configs { get; set; } = new();
        public List<System.Tuple<string, string>> LinkOptions { get; set; } = new();

        public EditModel(IDataService dataService, RootService rootService, 
            Microsoft.Extensions.Caching.Memory.IMemoryCache cache, PermissionService permissionService)
            : base(dataService, rootService, cache, permissionService)
        {
        }

        public IActionResult OnGet(int? id)
        {
            if (!id.HasValue)
            {
                return Redirect("/admin/page");
            }

            Page = Db.GetOne<PP_Page>(id.Value);
            if (Page == null)
            {
                return Redirect("/admin/page");
            }

            PageTemplates = Db.GetList<PP_Compt>(t => t.ComptType == "page_template")
                .OrderBy(t => t.ComptKey)
                .ToDictionary(t => t.ComptKey, t => t.ComptName ?? "");

            Components = Db.GetList<PP_Compt>(t => t.ComptKey != null && t.ComptKey.StartsWith(Page.ComptKey ?? ""));
            Configs = Db.GetList<PP_Config>(t => t.LangId == LangIdCompose && t.PageId == Page.Id);
            LinkOptions = Db.GetLinks(LangIdCompose);

            return Page();
        }

        public IActionResult OnPostUpdatePage([FromForm] int Id, [FromForm] string Title, 
            [FromForm] string PathPattern, [FromForm] string? MetaDescription, 
            [FromForm] string? MetaKeywords, [FromForm] string? ComptKey)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Title) || string.IsNullOrWhiteSpace(PathPattern))
                {
                    return new JsonResult(new { success = false, message = "Tiêu đề và đường dẫn không được để trống!" });
                }

                var page = Db.GetOne<PP_Page>(Id);
                if (page == null)
                {
                    return new JsonResult(new { success = false, message = "Không tìm thấy trang!" });
                }

                var compt = Db.GetOne<PP_Compt>(t => t.ComptKey == (ComptKey ?? page.ComptKey));
                if (compt == null)
                {
                    return new JsonResult(new { success = false, message = "Component không tồn tại!" });
                }

                var tempAlias = StringHelper.GetBeforeLast(PathPattern, "/") + compt.PathPostfix;
                if (tempAlias != page.PathPattern && Db.GetList<PP_Page>(t => t.PathPattern == tempAlias).Any())
                {
                    return new JsonResult(new { success = false, message = $"Đường dẫn [{tempAlias}] đã tồn tại!" });
                }

                page.Title = Title;
                page.PathPattern = tempAlias;
                page.MetaDescription = MetaDescription;
                page.MetaKeywords = MetaKeywords;
                page.ComptName = compt.ComptName;
                if (!string.IsNullOrEmpty(ComptKey))
                {
                    page.ComptKey = ComptKey;
                }

                Db.Update(page);
                Root.ClearCache();

                return new JsonResult(new { success = true, message = "Cập nhật thành công!", redirect = "/admin/page" });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }

        public IActionResult OnPostUpdateCompt([FromForm] string? action, [FromForm] string? langId,
            [FromForm] int? id, [FromForm] string? comptKey, [FromForm] string? jsonData)
        {
            try
            {
                if (!id.HasValue)
                {
                    return new JsonResult(new { success = false, message = "Không tìm thấy trang!" });
                }

                var page = Db.GetOne<PP_Page>(id.Value);
                if (page == null)
                {
                    return new JsonResult(new { success = false, message = "Không tìm thấy trang!" });
                }

                // Validate JSON
                JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonData ?? "{}");

                var config = Db.GetOne<PP_Config>(t => t.LangId == (langId ?? LangIdCompose)
                    && t.PageId == page.Id
                    && t.ConfigKey == comptKey);

                if (config != null)
                {
                    if (action == "reset")
                    {
                        var component = Db.GetOne<PP_Compt>(t => t.ComptKey == comptKey);
                        if (component != null)
                        {
                            config.JsonContent = component.JsonDefault;
                            Db.Update(config);
                        }
                    }
                    else
                    {
                        config.JsonContent = jsonData;
                        Db.Update(config);
                    }
                    Root.RefreshConfigs();
                }
                else
                {
                    config = new PP_Config
                    {
                        LangId = langId ?? LangIdCompose,
                        PageId = page.Id,
                        ConfigKey = comptKey,
                        JsonContent = jsonData
                    };

                    Db.Insert(config);
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

