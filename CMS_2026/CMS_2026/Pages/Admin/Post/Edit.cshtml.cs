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
    public class EditModel : BaseAdminPageModel
    {
        public PP_Node? Post { get; set; }
        public Dictionary<string, string> GroupSelector { get; set; } = new();

        public EditModel(IDataService dataService, RootService rootService, 
            Microsoft.Extensions.Caching.Memory.IMemoryCache cache, PermissionService permissionService)
            : base(dataService, rootService, cache, permissionService)
        {
        }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (!id.HasValue)
            {
                return Redirect("/admin/post");
            }

            Post = await Db.GetOneAsync<PP_Node>(id.Value);
            if (Post == null)
            {
                return Redirect("/admin/post");
            }

            GroupSelector = await GetGroupSelectorAsync(LangIdCompose, "post");

            return Page();
        }

        public async Task<IActionResult> OnPostAsync([FromForm] int Id, [FromForm] int CategoryId, 
            [FromForm] string Title, [FromForm] string NodePath, [FromForm] string? Summary, 
            [FromForm] string? Content, [FromForm] string? ImageUrl, [FromForm] bool Featured,
            [FromForm] string? MetaDescription, [FromForm] string? MetaKeywords)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Title) || string.IsNullOrWhiteSpace(NodePath) || CategoryId == 0)
                {
                    return new JsonResult(new { success = false, message = "Vui lòng điền đầy đủ thông tin!" });
                }

                var post = await Db.GetOneAsync<PP_Node>(Id);
                if (post == null)
                {
                    return new JsonResult(new { success = false, message = "Không tìm thấy bài viết!" });
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

                var existingNodes = await Db.GetListAsync<PP_Node>(t => t.NodePath == tempAlias && t.LangId == LangIdCompose && t.Id != Id);
                if (tempAlias != post.NodePath && existingNodes.Any())
                {
                    return new JsonResult(new { success = false, message = $"Đường dẫn [{tempAlias}] đã tồn tại!" });
                }

                post.CategoryId = CategoryId;
                post.Title = Title;
                post.NodePath = tempAlias;
                post.Summary = Summary;
                post.Content = Content;
                post.ImageUrl = ImageUrl;
                post.Featured = Featured;
                post.MetaDescription = MetaDescription;
                post.MetaKeywords = MetaKeywords;

                await Db.UpdateAsync(post);

                // Update category details
                var catDetails = await Db.GetOneAsync<PP_Category_details>(t => t.Idproduct == Id && t.NodeType == "post");
                if (catDetails != null)
                {
                    catDetails.Idcat = CategoryId;
                    await Db.UpdateAsync(catDetails);
                }
                else
                {
                    catDetails = new PP_Category_details
                    {
                        Idcat = CategoryId,
                        LangId = LangIdCompose,
                        PageId = category.PageId,
                        NodeType = "post",
                        PageIdItem = category.PageIdItem,
                        Idproduct = post.Id
                    };
                    await Db.InsertAsync(catDetails);
                }

                Root.ClearCache();

                return new JsonResult(new { success = true, message = "Cập nhật thành công!", redirect = "/admin/post" });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }
    }
}

