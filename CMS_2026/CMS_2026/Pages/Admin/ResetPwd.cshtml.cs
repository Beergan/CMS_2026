using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CMS_2026.Data;
using CMS_2026.Data.Entities;
using CMS_2026.Utils;
using CMS_2026.Common;
using CMS_2026.Services;

namespace CMS_2026.Pages.Admin
{
    public class ResetPwdModel : PageModel
    {
        private readonly IDataService _dataService;

        public ResetPwdModel(IDataService dataService)
        {
            _dataService = dataService;
        }

        [BindProperty]
        public string? NewPassword { get; set; }

        [BindProperty]
        public string? ConfirmationPassword { get; set; }

        public string? Message { get; set; }
        public bool IsSuccess { get; set; }

        public void OnGet(string? token, string? userid, string? success = null)
        {
            if (success == "success")
            {
                IsSuccess = true;
                Message = "Đổi mật khẩu thành công!";
            }
            else if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(userid))
            {
                Message = "Token hoặc UserId không hợp lệ!";
            }
        }

        public IActionResult OnPost(string? token, string? userid)
        {
            if (string.IsNullOrWhiteSpace(NewPassword))
            {
                ModelState.AddModelError("NewPassword", "Mật khẩu không được để trống.");
                return Page();
            }

            if (string.IsNullOrWhiteSpace(ConfirmationPassword))
            {
                ModelState.AddModelError("ConfirmationPassword", "Xác nhận mật khẩu không được để trống.");
                return Page();
            }

            // Validate password format: 6-15 characters, at least one uppercase, one lowercase, one digit
            if (!System.Text.RegularExpressions.Regex.IsMatch(NewPassword, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@#$%^&+=!])[A-Za-z\d@#$%^&+=!]{6,15}$"))
            {
                ModelState.AddModelError("NewPassword", "Mật khẩu từ 6-15 kí tự, phải có ít nhất một chữ hoa, chữ thường và một chữ số!");
                return Page();
            }

            if (NewPassword != ConfirmationPassword)
            {
                ModelState.AddModelError("ConfirmationPassword", "Xác nhận mật khẩu không khớp.");
                return Page();
            }

            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(userid))
            {
                ModelState.AddModelError("", "Token hoặc UserId không hợp lệ!");
                return Page();
            }

            var user = _dataService.GetOne<PP_User>(t => t.UserId == userid);
            if (user == null)
            {
                ModelState.AddModelError("", "Tài khoản không tồn tại!");
                return Page();
            }

            bool validateToken = JwtTokenHelper.ValidateToken(token, userid, user.Email ?? "");
            if (!validateToken)
            {
                ModelState.AddModelError("", "Liên kết đổi mật khẩu hết hạn hoặc không tồn tại!");
                return Page();
            }

            // Update password
            user.Password = CryptographyHelper.HashSHA256(NewPassword);
            _dataService.Update(user);
            _dataService.SaveChanges();

            return RedirectToPage("/Admin/ResetPwd", new { success = "success" });
        }
    }
}

