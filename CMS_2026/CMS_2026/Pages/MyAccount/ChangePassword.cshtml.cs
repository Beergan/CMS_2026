using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CMS_2026.Data.Entities;
using CMS_2026.Data;
using CMS_2026.Utils;
using CMS_2026.Services;

namespace CMS_2026.Pages.MyAccount
{
    public class ChangePasswordModel : PageModel
    {
        private readonly IDataService _db;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ChangePasswordModel(IDataService db, IHttpContextAccessor httpContextAccessor)
        {
            _db = db;
            _httpContextAccessor = httpContextAccessor;
        }

        [BindProperty]
        public string? CurrentPassword { get; set; }

        [BindProperty]
        public string? NewPassword { get; set; }

        [BindProperty]
        public string? ConfirmPassword { get; set; }

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
            if (string.IsNullOrWhiteSpace(CurrentPassword))
            {
                TempData["Error"] = "Vui lòng nhập mật khẩu hiện tại.";
                return RedirectToPage("/Components/myacount");
            }

            if (string.IsNullOrWhiteSpace(NewPassword) || NewPassword.Length < 6)
            {
                TempData["Error"] = "Mật khẩu mới phải có ít nhất 6 ký tự.";
                return RedirectToPage("/Components/myacount");
            }

            if (NewPassword != ConfirmPassword)
            {
                TempData["Error"] = "Mật khẩu mới và xác nhận mật khẩu không khớp.";
                return RedirectToPage("/Components/myacount");
            }

            // Check current password
            var currentPasswordHash = CryptographyHelper.HashSHA256(CurrentPassword);
            if (customer.PASSWORD != currentPasswordHash)
            {
                TempData["Error"] = "Mật khẩu hiện tại không đúng.";
                return RedirectToPage("/Components/myacount");
            }

            // Update password
            customer.PASSWORD = CryptographyHelper.HashSHA256(NewPassword);
            customer.CreatedTime = DateTime.Now;

            _db.Update(customer);
            _db.SaveChanges();

            TempData["Success"] = "Đổi mật khẩu thành công!";
            return RedirectToPage("/Components/myacount");
        }
    }
}

