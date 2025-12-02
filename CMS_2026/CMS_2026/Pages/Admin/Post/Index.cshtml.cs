using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CMS_2026.Data.Entities;
using CMS_2026.Services;

namespace CMS_2026.Pages.Admin.Post
{
    public class IndexModel : BaseAdminPageModel
    {
        public List<PP_Node> Posts { get; set; } = new();
        public PP_Category? Category { get; set; }
        public int CatId { get; set; }

        public IndexModel(IDataService dataService, RootService rootService, 
            Microsoft.Extensions.Caching.Memory.IMemoryCache cache, PermissionService permissionService)
            : base(dataService, rootService, cache, permissionService)
        {
        }

        public void OnGet(int? catId = null)
        {
            CatId = catId ?? 0;
            var query = Db.GetList<PP_Node>(t => t.LangId == LangIdCompose && t.NodeType == "post");

            if (catId.HasValue && catId.Value > 0)
            {
                Category = Db.GetOne<PP_Category>(catId.Value);
                query = query.Where(t => t.CategoryId == catId.Value).ToList();
            }

            Posts = query
                .OrderByDescending(t => t.CreatedTime)
                .ToList();
        }

        public IActionResult OnPostDelete([FromForm] int Id)
        {
            try
            {
                var item = Db.GetOne<PP_Node>(Id);
                if (item == null)
                {
                    return new JsonResult(new { success = false, message = "Không tìm thấy bài viết!" });
                }

                Db.Delete<PP_Node>(item.Id);
                return new JsonResult(new { success = true, message = $"Mục [{item.Title}] đã được xóa!" });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }
    }
}

