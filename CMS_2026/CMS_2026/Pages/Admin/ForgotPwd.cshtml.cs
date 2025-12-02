using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CMS_2026.Data;
using CMS_2026.Data.Entities;
using CMS_2026.Services;
using CMS_2026.Utils;
using CMS_2026.Common;

namespace CMS_2026.Pages.Admin
{
    public class ForgotPwdModel : PageModel
    {
        private readonly IDataService _dataService;
        private readonly RootService _rootService;
        private readonly EmailService _emailService;

        public ForgotPwdModel(IDataService dataService, RootService rootService, EmailService emailService)
        {
            _dataService = dataService;
            _rootService = rootService;
            _emailService = emailService;
        }

        [BindProperty]
        public string? UserId { get; set; }

        public string? Message { get; set; }
        public bool IsSuccess { get; set; }

        public void OnGet(string? success = null)
        {
            if (success == "success")
            {
                IsSuccess = true;
                Message = "Hãy vào Email để xác nhận lấy liên kết đổi mật khẩu mới";
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrWhiteSpace(UserId))
            {
                ModelState.AddModelError("UserId", "Tên đăng nhập không được để trống.");
                return Page();
            }

            var user = _dataService.GetOne<PP_User>(t => t.UserId == UserId);
            if (user == null)
            {
                ModelState.AddModelError("UserId", "Tài khoản không tồn tại!");
                return Page();
            }

            if (string.IsNullOrEmpty(user.Email))
            {
                ModelState.AddModelError("UserId", "Tài khoản chưa có email!");
                return Page();
            }

            try
            {
                string token = JwtTokenHelper.GeneratePasswordResetToken(user.UserId, user.Email);
                string url = $"{_rootService.CurrentWebsiteUrl}{Constants.Admin_Url}/resetpwd?token={token}&userid={user.UserId}";
                string content = $"<p>Xin chào: {user.DisplayName}<p>";
                content += $"<p>Để tạo mật khẩu mới, bạn hãy click vào liên kết dưới đây:</p>";
                content += $"<p>Vui lòng Click vào đây: <a href='{url}'><b>Tạo mật khẩu mới</b></a></p>";
                content += $"<p><b>Liên kết có hiệu lực trong 15 phút</b></p>";
                content += $"<p>Nếu bạn không yêu cầu điều này, chỉ cần bỏ qua thông báo này.<p>";

                var result = await _emailService.SendEmailAsync(user.Email, "Thông tin đặt lại mật khẩu", content);
                if (result)
                {
                    return RedirectToPage("/Admin/ForgotPwd", new { success = "success" });
                }
                else
                {
                    ModelState.AddModelError("UserId", "Không thể gửi email. Vui lòng thử lại sau.");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("UserId", $"Lỗi: {ex.Message}");
            }

            return Page();
        }
    }
}

