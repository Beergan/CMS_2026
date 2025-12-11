using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CMS_2026.Data.Entities;
using CMS_2026.Services;
using CMS_2026.Common;

namespace CMS_2026.Pages.Admin.Category
{
    public class IndexModel : BaseAdminPageModel
    {
        public List<PP_Category> Categories { get; set; } = new();
        public string NodeType { get; set; } = string.Empty;

        public IndexModel(IDataService dataService, RootService rootService, 
            Microsoft.Extensions.Caching.Memory.IMemoryCache cache, PermissionService permissionService)
            : base(dataService, rootService, cache, permissionService)
        {
        }

        public async Task OnGetAsync(string? type)
        {
            NodeType = type ?? string.Empty;
            
            if (!string.IsNullOrEmpty(NodeType) && !Constants.GroupTypeOptionsVI.ContainsKey(NodeType))
            {
                NodeType = string.Empty;
            }

            var query = await Db.GetListAsync<PP_Category>(t => t.LangId == LangIdCompose);

            if (!string.IsNullOrEmpty(NodeType))
            {
                query = query.Where(t => t.NodeType == NodeType).ToList();
            }

            Categories = query.OrderBy(t => t.Breadcrumb).ToList();
        }

        public async Task<IActionResult> OnPostDeleteAsync([FromForm] int Id)
        {
            try
            {
                var item = await Db.GetOneAsync<PP_Category>(Id);
                if (item == null)
                {
                    return new JsonResult(new { success = false, message = "Không tìm thấy chuyên mục!" });
                }

                var childCategories = await Db.GetListAsync<PP_Category>(t => t.ParentId == item.Id);
                if (childCategories.Any())
                {
                    return new JsonResult(new { success = false, message = "Không thể xóa, vẫn còn nội dung tham chiếu đến mục này!" });
                }

                var nodes = await Db.GetListAsync<PP_Node>(t => t.CategoryId == item.Id);
                if (nodes.Any())
                {
                    return new JsonResult(new { success = false, message = "Không thể xóa, vẫn còn nội dung tham chiếu đến mục này!" });
                }

                await Db.DeleteAsync<PP_Category>(item.Id);
                Root.ClearCache();
                return new JsonResult(new { success = true, message = $"Mục [{item.Title}] đã được xóa!" });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }
    }
}
