using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CMS_2026.Data.Entities;
using CMS_2026.Services;
using CMS_2026.Pages.Admin;
using CMS_2026.Utils;

namespace CMS_2026.Pages.Admin.Post
{
    public class ImageModel : BaseAdminPageModel
    {
        public PP_Node? Post { get; set; }

        public ImageModel(IDataService dataService, RootService rootService, 
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

            return Page();
        }

        public async Task<IActionResult> OnPostAsync([FromForm] int Id, [FromForm] string ImageUrl)
        {
            try
            {
                var post = await Db.GetOneAsync<PP_Node>(Id);
                if (post == null)
                {
                    return new JsonResult(new { success = false, message = "Bài viết không tồn tại!" });
                }

                post.ImageUrl = ImageUrl.NullIfWhiteSpace();
                await Db.UpdateAsync(post);
                Root.ClearCache();

                return new JsonResult(new { success = true, message = "Cập nhật hình ảnh thành công!" });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }
    }
}

