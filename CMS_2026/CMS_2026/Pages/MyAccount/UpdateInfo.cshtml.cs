using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CMS_2026.Data.Entities;
using CMS_2026.Data;
using CMS_2026.Utils;
using CMS_2026.Services;

namespace CMS_2026.Pages.MyAccount
{
    public class UpdateInfoModel : PageModel
    {
        private readonly IDataService _db;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UpdateInfoModel(IDataService db, IHttpContextAccessor httpContextAccessor)
        {
            _db = db;
            _httpContextAccessor = httpContextAccessor;
        }

        [BindProperty]
        public string? Name { get; set; }

        [BindProperty]
        public string? Phone { get; set; }

        [BindProperty]
        public string? Address { get; set; }

        public IActionResult OnPost()
        {
            var customerId = HttpContext.Session.GetCustomerId();
            if (customerId == null)
            {
                TempData["Error"] = "Vui lòng đăng nhập để tiếp tục.";
                return RedirectToPage("/Components/login");
            }

            var customer = _db.GetOne<PP_Register>(customerId.Value);
            if (customer == null)
            {
                TempData["Error"] = "Không tìm thấy thông tin khách hàng.";
                return RedirectToPage("/Components/login");
            }

            // Validate
            if (string.IsNullOrWhiteSpace(Name))
            {
                TempData["Error"] = "Vui lòng nhập họ và tên.";
                return RedirectToPage("/Components/myacount");
            }

            // Update customer info
            customer.Name = Name?.Trim();
            customer.Phone = Phone?.NullIfWhiteSpace();
            customer.Address = Address?.NullIfWhiteSpace();
            customer.CreatedTime = DateTime.Now;

            _db.Update(customer);
            _db.SaveChanges();

            TempData["Success"] = "Cập nhật thông tin thành công!";
            return RedirectToPage("/Components/myacount");
        }
    }
}

