using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CMS_2026.Data.Entities;
using CMS_2026.Services;
using CMS_2026.Utils;

namespace CMS_2026.Pages.Admin.Post
{
    public class CreateModel : BaseAdminPageModel
    {
        public Dictionary<string, string> GroupSelector { get; set; } = new();
        [BindProperty]
        public string ContentJson { get; set; }
        public CreateModel(IDataService dataService, RootService rootService, 
            Microsoft.Extensions.Caching.Memory.IMemoryCache cache, PermissionService permissionService)
            : base(dataService, rootService, cache, permissionService)
        {
        }

        public async Task OnGetAsync(int? categoryId = null)
        {
            GroupSelector = await GetGroupSelectorAsync(LangIdCompose, "post");
        }

        public async Task<IActionResult> OnPostAsync([FromForm] int CategoryId, [FromForm] string Title, 
            [FromForm] string NodePath, [FromForm] string? Summary, [FromForm] string? Content,
            [FromForm] string? ImageUrl, [FromForm] bool Featured, 
            [FromForm] string? MetaDescription, [FromForm] string? MetaKeywords)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Title) || string.IsNullOrWhiteSpace(NodePath) || CategoryId == 0)
                {
                    return new JsonResult(new { success = false, message = "Vui lòng điền đầy đủ thông tin!" });
                }

                var category = await Db.GetOneAsync<PP_Category>(CategoryId);
                if (category == null)
                {
                    return new JsonResult(new { success = false, message = "Chuyên mục không tồn tại!" });
                }

                var page = await Db.GetOneAsync<PP_Page>(category.PageIdItem);
                if (page == null)
                {
                    return new JsonResult(new { success = false, message = "Trang chi tiết không tồn tại!" });
                }

                var slug = EncodeHelper.SanitizeString(NodePath);
                var tempAlias = string.Format(page.PathPattern, slug);

                var existingNodes = await Db.GetListAsync<PP_Node>(t => t.NodePath == tempAlias && t.LangId == LangIdCompose);
                if (existingNodes.Any())
                {
                    return new JsonResult(new { success = false, message = $"Đường dẫn [{tempAlias}] đã tồn tại!" });
                }

                var post = new PP_Node
                {
                    LangId = LangIdCompose,
                    NodeType = "post",
                    CategoryId = CategoryId,
                    Title = Title,
                    NodePath = tempAlias,
                    Summary = Summary,
                    Content = Content,
                    ImageUrl = ImageUrl,
                    Featured = Featured,
                    MetaDescription = MetaDescription,
                    MetaKeywords = MetaKeywords,
                    NodeStatus = "CREATED",
                    PageId = category.PageId,
                    PageIdItem = category.PageIdItem
                };

                await Db.InsertAsync(post);

                // Create category details
                var catDetails = new PP_Category_details
                {
                    Idcat = CategoryId,
                    LangId = LangIdCompose,
                    PageId = category.PageId,
                    NodeType = "post",
                    PageIdItem = category.PageIdItem,
                    Idproduct = post.Id
                };
                await Db.InsertAsync(catDetails);

                Root.ClearCache(); 

                return new JsonResult(new { success = true, message = "Tạo bài viết thành công!", redirect = "/admin/post" });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }
    }
}

