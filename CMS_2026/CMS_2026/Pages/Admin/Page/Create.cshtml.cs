using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CMS_2026.Data.Entities;
using CMS_2026.Services;
using CMS_2026.Utils;

namespace CMS_2026.Pages.Admin.Page
{
    public class CreateModel : BaseAdminPageModel
    {
        public Dictionary<string, string> PageTemplates { get; set; } = new();

        public CreateModel(IDataService dataService, RootService rootService, 
            Microsoft.Extensions.Caching.Memory.IMemoryCache cache, PermissionService permissionService)
            : base(dataService, rootService, cache, permissionService)
        {
        }

        public void OnGet()
        {
            PageTemplates = Db.GetList<PP_Compt>(t => t.ComptType == "page_template")
                .OrderBy(t => t.ComptKey)
                .ToDictionary(t => t.ComptKey, t => t.ComptName);
        }

        public IActionResult OnPost([FromForm] string Title, [FromForm] string PathPattern,
            [FromForm] string? MetaDescription, [FromForm] string? MetaKeywords, [FromForm] string ComptKey)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Title) || string.IsNullOrWhiteSpace(PathPattern) || string.IsNullOrWhiteSpace(ComptKey))
                {
                    return new JsonResult(new { success = false, message = "Vui lòng điền đầy đủ thông tin!" });
                }

                var compt = Db.GetOne<PP_Compt>(t => t.ComptKey == ComptKey);
                if (compt == null)
                {
                    return new JsonResult(new { success = false, message = "Component không tồn tại!" });
                }

                var tempAlias = PathPattern.TrimStart('/').TrimEnd('/') + compt.PathPostfix;
                if (Db.GetList<PP_Page>(t => t.PathPattern == tempAlias && t.LangId == LangIdCompose).Any())
                {
                    return new JsonResult(new { success = false, message = $"Đường dẫn [{tempAlias}] đã tồn tại!" });
                }

                var page = new PP_Page
                {
                    LangId = LangIdCompose,
                    Title = Title,
                    PathPattern = tempAlias,
                    ComptKey = ComptKey,
                    ComptName = compt.ComptName,
                    MetaDescription = MetaDescription,
                    MetaKeywords = MetaKeywords,
                    PageType = compt.PageType,
                    NodeType = compt.NodeType
                };

                Db.Insert(page);
                Root.ClearCache();

                return new JsonResult(new { success = true, message = "Tạo trang thành công!", redirect = "/admin/page" });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }
    }
}

