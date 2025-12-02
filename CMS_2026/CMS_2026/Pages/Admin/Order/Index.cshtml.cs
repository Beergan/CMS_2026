using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CMS_2026.Data.Entities;
using CMS_2026.Services;

namespace CMS_2026.Pages.Admin.Order
{
    public class IndexModel : BaseAdminPageModel
    {
        public List<PP_Order> Orders { get; set; } = new();
        public string Status { get; set; } = string.Empty;

        public IndexModel(IDataService dataService, RootService rootService, 
            Microsoft.Extensions.Caching.Memory.IMemoryCache cache, PermissionService permissionService)
            : base(dataService, rootService, cache, permissionService)
        {
        }

        public void OnGet(string? newStatus, string? delivering, string? cancelled, string? success, string? all)
        {
            var query = Db.GetList<PP_Order>();

            if (!string.IsNullOrEmpty(newStatus))
            {
                Status = "NEW";
                query = query.Where(t => t.OrderStatus == "NEW").ToList();
            }
            else if (!string.IsNullOrEmpty(delivering))
            {
                Status = "DELIVERING";
                query = query.Where(t => t.OrderStatus == "DELIVERING").ToList();
            }
            else if (!string.IsNullOrEmpty(cancelled))
            {
                Status = "CANCELLED";
                query = query.Where(t => t.OrderStatus == "CANCELLED").ToList();
            }
            else if (!string.IsNullOrEmpty(success))
            {
                Status = "SUCCESS";
                query = query.Where(t => t.OrderStatus == "SUCCESS").ToList();
            }
            else if (!string.IsNullOrEmpty(all))
            {
                Status = "all";
            }
            else
            {
                Status = "NEW";
                query = query.Where(t => t.OrderStatus == "NEW").ToList();
            }

            Orders = query
                .OrderByDescending(t => t.CreatedTime)
                .ToList();
        }

        public IActionResult OnPostUpdateStatus([FromForm] int id, [FromForm] string status, [FromForm] string? note)
        {
            try
            {
                var order = Db.GetOne<PP_Order>(id);
                if (order == null)
                {
                    return new JsonResult(new { success = false, message = "Không tìm thấy đơn hàng!" });
                }

                order.OrderStatus = status;
                if (!string.IsNullOrEmpty(note))
                {
                    order.Note = note;
                }
                Db.Update(order);

                return new JsonResult(new { success = true, message = "Thông tin đã được ghi nhận!" });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }
    }
}

