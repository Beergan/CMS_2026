using System.Collections.Generic;
using System.Linq;
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

        public void OnGet(int? categoryId = null)
        {
            GroupSelector = GetGroupSelector(LangIdCompose, "post");
        }

        public IActionResult OnPost([FromForm] int CategoryId, [FromForm] string Title, 
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

                var category = Db.GetOne<PP_Category>(CategoryId);
                if (category == null)
                {
                    return new JsonResult(new { success = false, message = "Chuyên mục không tồn tại!" });
                }

                var page = Db.GetOne<PP_Page>(category.PageIdItem);
                if (page == null)
                {
                    return new JsonResult(new { success = false, message = "Trang chi tiết không tồn tại!" });
                }

                var slug = EncodeHelper.SanitizeString(NodePath);
                var tempAlias = string.Format(page.PathPattern, slug);

                if (Db.GetList<PP_Node>(t => t.NodePath == tempAlias && t.LangId == LangIdCompose).Any())
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

                Db.Insert(post);

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
                Db.Insert(catDetails);

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

