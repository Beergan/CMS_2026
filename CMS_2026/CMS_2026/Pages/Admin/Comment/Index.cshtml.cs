using System.Collections.Generic;
using System.Linq;
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

        public void OnGet(string? newStatus, string? processing, string? cancelled, string? success)
        {
            var query = Db.GetList<PP_Comment>();

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

        public IActionResult OnPostUpdateStatus([FromForm] int id, [FromForm] string status, [FromForm] string? note)
        {
            try
            {
                var comment = Db.GetOne<PP_Comment>(id);
                if (comment == null)
                {
                    return new JsonResult(new { success = false, message = "Không tìm thấy comment!" });
                }

                comment.Status = status;
                if (!string.IsNullOrEmpty(note))
                {
                    comment.ProcessNote = note;
                }
                Db.Update(comment);

                return new JsonResult(new { success = true, message = "Thông tin đã được ghi nhận!" });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }
    }
}

