using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CMS_2026.Data.Entities;
using CMS_2026.Services;
using Newtonsoft.Json;
using CMS_2026.Utils;

namespace CMS_2026.Pages.Admin.Category
{
    public class CreateModel : BaseAdminPageModel
    {
        public Dictionary<string, string> GroupSelector { get; set; } = new();
        public Dictionary<string, string> PageSelections { get; set; } = new();
        public Dictionary<string, string> PageItemSelections { get; set; } = new();
        public string NodeType { get; set; } = string.Empty;

        public CreateModel(IDataService dataService, RootService rootService, 
            Microsoft.Extensions.Caching.Memory.IMemoryCache cache, PermissionService permissionService)
            : base(dataService, rootService, cache, permissionService)
        {
        }

        public async Task OnGetAsync(string? type)
        {
            NodeType = type ?? string.Empty;
            
            GroupSelector = await GetGroupSelectorAsync(LangIdCompose, NodeType);
            
            var pages = await Db.GetListAsync<PP_Page>(t => t.LangId == LangIdCompose 
                && t.PageType == "list" 
                && (string.IsNullOrEmpty(NodeType) || t.NodeType == NodeType));
            PageSelections = pages.ToDictionary(t => t.Id.ToString(), t => t.Title);

            var pageItems = await Db.GetListAsync<PP_Page>(t => t.LangId == LangIdCompose && t.PageType == "item");
            PageItemSelections = pageItems.ToDictionary(t => t.Id.ToString(), t => t.Title);
        }

        public async Task<IActionResult> OnPostAsync([FromForm] int? ParentId, [FromForm] string Title, 
            [FromForm] string CategoryPath, [FromForm] int PageId, [FromForm] int PageIdItem,
            [FromForm] string? ImageUrl, [FromForm] string? MetaDescription, 
            [FromForm] string? MetaKeywords, [FromForm] string? NodeType)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Title) || string.IsNullOrWhiteSpace(CategoryPath) || PageId == 0)
                {
                    return new JsonResult(new { success = false, message = "Vui lòng điền đầy đủ thông tin!" });
                }

                var page = await Db.GetOneAsync<PP_Page>(PageId);
                if (page == null)
                {
                    return new JsonResult(new { success = false, message = "Trang không tồn tại!" });
                }

                var category = new PP_Category
                {
                    LangId = LangIdCompose,
                    Title = Title,
                    NodeType = NodeType ?? "post",
                    PageId = PageId,
                    PageIdItem = PageIdItem,
                    ImageUrl = ImageUrl,
                    MetaDescription = MetaDescription,
                    MetaKeywords = MetaKeywords
                };

                List<KeyValuePair<string, string>> breadcrumbs = new();

                if (ParentId.HasValue && ParentId.Value > 0)
                {
                    var parentCategory = await Db.GetOneAsync<PP_Category>(ParentId.Value);
                    if (parentCategory == null)
                    {
                        return new JsonResult(new { success = false, message = "Chuyên mục cha không tồn tại!" });
                    }

                    category.ParentId = parentCategory.Id;
                    category.CategoryLevel = parentCategory.CategoryLevel + 1;
                    category.CategoryPath = string.Format(page.PathPattern, CategoryPath.Trim());
                    breadcrumbs = JsonConvert.DeserializeObject<List<KeyValuePair<string, string>>>(parentCategory.Breadcrumb) ?? new();
                }
                else
                {
                    category.CategoryPath = string.Format(page.PathPattern.GetBeforeLast("/"), CategoryPath.Trim());
                    category.CategoryLevel = 1;
                }

                var existingCategories = await Db.GetListAsync<PP_Category>(t => t.LangId == LangIdCompose && t.CategoryPath == category.CategoryPath);
                if (existingCategories.Any())
                {
                    return new JsonResult(new { success = false, message = "Đường dẫn đã tồn tại!" });
                }

                breadcrumbs.Add(new KeyValuePair<string, string>(category.CategoryPath, category.Title));
                category.Breadcrumb = JsonConvert.SerializeObject(breadcrumbs);

                await Db.InsertAsync(category);
                await Root.RefreshCategoryIndexesAsync();
                Root.ClearCache();

                return new JsonResult(new { success = true, message = "Tạo chuyên mục thành công!", redirect = $"/admin/category?type={NodeType}" });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }
    }
}

