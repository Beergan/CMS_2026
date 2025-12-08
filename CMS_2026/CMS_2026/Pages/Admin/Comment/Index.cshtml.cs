using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CMS_2026.Data.Entities;
using CMS_2026.Services;

namespace CMS_2026.Pages.Admin.Comment
{
    public class IndexModel : BaseAdminPageModel
    {
        public List<PP_Comment> Comments { get; set; } = new();
        public string Status { get; set; } = string.Empty;

        public IndexModel(IDataService dataService, RootService rootService, 
            Microsoft.Extensions.Caching.Memory.IMemoryCache cache, PermissionService permissionService)
            : base(dataService, rootService, cache, permissionService)
        {
        }

        public async Task OnGetAsync(string? newStatus, string? processing, string? cancelled, string? success)
        {
            var query = await Db.GetListAsync<PP_Comment>();

            if (!string.IsNullOrEmpty(newStatus))
            {
                Status = "NEW";
                query = query.Where(t => t.Status == "NEW").ToList();
            }
            else if (!string.IsNullOrEmpty(processing))
            {
                Status = "PROCESSING";
                query = query.Where(t => t.Status == "PROCESSING").ToList();
            }
            else if (!string.IsNullOrEmpty(cancelled))
            {
                Status = "CANCELLED";
                query = query.Where(t => t.Status == "CANCELLED").ToList();
            }
            else if (!string.IsNullOrEmpty(success))
            {
                Status = "SUCCESS";
                query = query.Where(t => t.Status == "SUCCESS").ToList();
            }
            else
            {
                Status = "NEW";
                query = query.Where(t => t.Status == "NEW").ToList();
            }

            Comments = query
                .OrderByDescending(t => t.CreatedTime)
                .ToList();
        }

        public async Task<IActionResult> OnPostUpdateStatusAsync([FromForm] int id, [FromForm] string status, [FromForm] string? note)
        {
            try
            {
                var comment = await Db.GetOneAsync<PP_Comment>(id);
                if (comment == null)
                {
                    return new JsonResult(new { success = false, message = "Không tìm thấy comment!" });
                }

                comment.Status = status;
                if (!string.IsNullOrEmpty(note))
                {
                    comment.ProcessNote = note;
                }
                await Db.UpdateAsync(comment);

                return new JsonResult(new { success = true, message = "Thông tin đã được ghi nhận!" });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }
    }
}

