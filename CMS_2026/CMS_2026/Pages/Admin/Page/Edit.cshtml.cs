using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (!id.HasValue)
            {
                return Redirect("/admin/page");
            }

            Page = await Db.GetOneAsync<PP_Page>(id.Value);
            if (Page == null)
            {
                return Redirect("/admin/page");
            }

            var pageTemplatesList = await Db.GetListAsync<PP_Compt>(t => t.ComptType ==  "page_template");
            PageTemplates = pageTemplatesList
                .OrderBy(t => t.ComptKey)
                .ToDictionary(t => t.ComptKey, t => t.ComptName ?? "");

            Components = await Db.GetListAsync<PP_Compt>(t => t.ComptKey != null && t.ComptKey.StartsWith(Page.ComptKey ?? ""));
            Configs = await Db.GetListAsync<PP_Config>(t => t.LangId == LangIdCompose && t.PageId == Page.Id);
            LinkOptions = await Db.GetLinksAsync(LangIdCompose);

            return Page();
        }

        public async Task<IActionResult> OnPostUpdatePageAsync([FromForm] int Id, [FromForm] string Title, 
            [FromForm] string PathPattern, [FromForm] string? MetaDescription, 
            [FromForm] string? MetaKeywords, [FromForm] string? ComptKey)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Title) || string.IsNullOrWhiteSpace(PathPattern))
                {
                    return new JsonResult(new { success = false, message = "Tiêu đề và đường dẫn không được để trống!" });
                }

                var page = await Db.GetOneAsync<PP_Page>(Id);
                if (page == null)
                {
                    return new JsonResult(new { success = false, message = "Không tìm thấy trang!" });
                }

                var compt = await Db.GetOneAsync<PP_Compt>(t => t.ComptKey == (ComptKey ?? page.ComptKey));
                if (compt == null)
                {
                    return new JsonResult(new { success = false, message = "Component không tồn tại!" });
                }

                var tempAlias = StringHelper.GetBeforeLast(PathPattern, "/") + compt.PathPostfix;
                var existingPages = await Db.GetListAsync<PP_Page>(t => t.PathPattern == tempAlias);
                if (tempAlias != page.PathPattern && existingPages.Any())
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

                await Db.UpdateAsync(page);
                Root.ClearCache();

                return new JsonResult(new { success = true, message = "Cập nhật thành công!", redirect = "/admin/page" });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }

        public async Task<IActionResult> OnPostUpdateComptAsync([FromForm] string? action, [FromForm] string? langId,
            [FromForm] int? id, [FromForm] string? comptKey, [FromForm] string? jsonData)
        {
            try
            {
                if (!id.HasValue)
                {
                    return new JsonResult(new { success = false, message = "Không tìm thấy trang!" });
                }

                var page = await Db.GetOneAsync<PP_Page>(id.Value);
                if (page == null)
                {
                    return new JsonResult(new { success = false, message = "Không tìm thấy trang!" });
                }

                // Validate JSON
                JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonData ?? "{}");

                var config = await Db.GetOneAsync<PP_Config>(t => t.LangId == (langId ?? LangIdCompose)
                    && t.PageId == page.Id
                    && t.ConfigKey == comptKey);

                if (config != null)
                {
                    if (action == "reset")
                    {
                        var component = await Db.GetOneAsync<PP_Compt>(t => t.ComptKey == comptKey);
                        if (component != null)
                        {
                            config.JsonContent = component.JsonDefault;
                            await Db.UpdateAsync(config);
                        }
                    }
                    else
                    {
                        config.JsonContent = jsonData;
                        await Db.UpdateAsync(config);
                    }
                    await Root.RefreshConfigsAsync();
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

                    await Db.InsertAsync(config);
                    await Root.RefreshConfigsAsync();
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

