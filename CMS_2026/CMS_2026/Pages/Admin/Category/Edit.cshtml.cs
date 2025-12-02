using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CMS_2026.Data.Entities;
using CMS_2026.Services;
using Newtonsoft.Json;
using CMS_2026.Utils;

namespace CMS_2026.Pages.Admin.Category
{
    public class EditModel : BaseAdminPageModel
    {
        public PP_Category? Category { get; set; }
        public Dictionary<string, string> GroupSelector { get; set; } = new();
        public Dictionary<string, string> PageSelections { get; set; } = new();
        public Dictionary<string, string> PageItemSelections { get; set; } = new();

        public EditModel(IDataService dataService, RootService rootService, 
            Microsoft.Extensions.Caching.Memory.IMemoryCache cache, PermissionService permissionService)
            : base(dataService, rootService, cache, permissionService)
        {
        }

        public IActionResult OnGet(int? id)
        {
            if (!id.HasValue)
            {
                return Redirect("/admin/category");
            }

            Category = Db.GetOne<PP_Category>(id.Value);
            if (Category == null)
            {
                return Redirect("/admin/category");
            }

            GroupSelector = GetGroupSelector(LangIdCompose, Category.NodeType);
            
            PageSelections = Db.GetList<PP_Page>(t => t.LangId == LangIdCompose 
                && t.PageType == "list" 
                && t.NodeType == Category.NodeType)
                .ToDictionary(t => t.Id.ToString(), t => t.Title);

            PageItemSelections = Db.GetList<PP_Page>(t => t.LangId == LangIdCompose && t.PageType == "item")
                .ToDictionary(t => t.Id.ToString(), t => t.Title);

            return Page();
        }

        public IActionResult OnPost([FromForm] int Id, [FromForm] int? ParentId, 
            [FromForm] string Title, [FromForm] string CategoryPath, [FromForm] int PageId, 
            [FromForm] int PageIdItem, [FromForm] string? ImageUrl, 
            [FromForm] string? MetaDescription, [FromForm] string? MetaKeywords)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Title) || string.IsNullOrWhiteSpace(CategoryPath) || PageId == 0)
                {
                    return new JsonResult(new { success = false, message = "Vui lòng điền đầy đủ thông tin!" });
                }

                var category = Db.GetOne<PP_Category>(Id);
                if (category == null)
                {
                    return new JsonResult(new { success = false, message = "Không tìm thấy chuyên mục!" });
                }

                var page = Db.GetOne<PP_Page>(PageId);
                if (page == null)
                {
                    return new JsonResult(new { success = false, message = "Trang không tồn tại!" });
                }

                List<KeyValuePair<string, string>> breadcrumbs = new();

                if (ParentId.HasValue && ParentId.Value > 0)
                {
                    if (ParentId.Value == Id)
                    {
                        return new JsonResult(new { success = false, message = "Thao tác không hợp lệ!" });
                    }

                    var parentCategory = Db.GetOne<PP_Category>(ParentId.Value);
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
                    category.ParentId = null;
                    category.CategoryLevel = 1;
                    category.CategoryPath = string.Format(page.PathPattern.GetBeforeLast("/"), CategoryPath.Trim());
                }

                if (category.CategoryPath != CategoryPath && 
                    Db.GetList<PP_Category>(t => t.LangId == LangIdCompose && t.CategoryPath == category.CategoryPath && t.Id != Id).Any())
                {
                    return new JsonResult(new { success = false, message = "Đường dẫn đã tồn tại!" });
                }

                category.Title = Title;
                category.PageId = PageId;
                category.PageIdItem = PageIdItem;
                category.ImageUrl = ImageUrl;
                category.MetaDescription = MetaDescription;
                category.MetaKeywords = MetaKeywords;

                breadcrumbs.Add(new KeyValuePair<string, string>(category.CategoryPath, category.Title));
                category.Breadcrumb = JsonConvert.SerializeObject(breadcrumbs);

                Db.Update(category);
                Root.RefreshCategoryIndexes();
                Root.ClearCache();

                return new JsonResult(new { success = true, message = "Cập nhật thành công!", redirect = "/admin/category" });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }
    }
}

