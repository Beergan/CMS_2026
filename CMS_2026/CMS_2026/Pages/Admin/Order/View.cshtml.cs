using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CMS_2026.Data.Entities;
using CMS_2026.Services;

namespace CMS_2026.Pages.Admin.Order
{
    public class ViewModel : BaseAdminPageModel
    {
        public PP_Order? Order { get; set; }

        public ViewModel(IDataService dataService, RootService rootService, 
            Microsoft.Extensions.Caching.Memory.IMemoryCache cache, PermissionService permissionService)
            : base(dataService, rootService, cache, permissionService)
        {
        }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (!id.HasValue)
            {
                return Redirect("/admin/order");
            }

            Order = await Db.GetOneAsync<PP_Order>(id.Value);
            if (Order == null)
            {
                return Redirect("/admin/order");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostUpdateStatusAsync([FromForm] int id, [FromForm] string status, [FromForm] string? note)
        {
            try
            {
                var order = await Db.GetOneAsync<PP_Order>(id);
                if (order == null)
                {
                    return new JsonResult(new { success = false, message = "Không tìm thấy đơn hàng!" });
                }

                order.OrderStatus = status;
                if (!string.IsNullOrEmpty(note))
                {
                    order.Note = note;
                }
                await Db.UpdateAsync(order);

                return new JsonResult(new { success = true, message = "Cập nhật thành công!", redirect = "/admin/order" });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }
    }
}

