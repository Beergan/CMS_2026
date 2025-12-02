using System;
using CMS_2026.Data.Entities;
using CMS_2026.Services;
using CMS_2026.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CMS_2026.Pages.Register
{
    public class ProcessModel : PageModel
    {
        private readonly IDataService _dataService;
        private readonly RootService _rootService;

        public ProcessModel(IDataService dataService, RootService rootService)
        {
            _dataService = dataService;
            _rootService = rootService;
        }

        public IActionResult OnPost([FromForm] string FullName, [FromForm] string Email, [FromForm] string Password, [FromForm] string ConfirmPassword, [FromForm] string? Phone)
        {
            var referer = Request.Headers["Referer"].ToString();
            var fallback = string.IsNullOrEmpty(referer) ? "/register" : referer;

            if (string.IsNullOrWhiteSpace(FullName) || string.IsNullOrWhiteSpace(Email) ||
                string.IsNullOrWhiteSpace(Password) || string.IsNullOrWhiteSpace(ConfirmPassword))
            {
                TempData["RegisterError"] = "Vui lòng điền đầy đủ thông tin.";
                return Redirect(fallback);
            }

            if (!string.Equals(Password, ConfirmPassword, StringComparison.Ordinal))
            {
                TempData["RegisterError"] = "Xác nhận mật khẩu không khớp.";
                return Redirect(fallback);
            }

            if (Password.Length < 6)
            {
                TempData["RegisterError"] = "Mật khẩu phải có ít nhất 6 ký tự.";
                return Redirect(fallback);
            }

            var existing = _dataService.GetOne<PP_Register>(x => x.Email == Email);
            if (existing != null)
            {
                TempData["RegisterError"] = "Email đã tồn tại.";
                return Redirect(fallback);
            }

            var customer = new PP_Register
            {
                Name = FullName,
                Email = Email,
                Phone = Phone,
                PASSWORD = CryptographyHelper.HashSHA256(Password),
                Status = "NEW",
                Active = true
            };

            _dataService.Insert(customer);
            HttpContext.Session.SignInCustomer(customer);
            TempData["RegisterSuccess"] = "Đăng ký tài khoản thành công!";

            var langId = Request.Cookies["LangId"] ?? "vi";
            var config = _rootService.GetConfig(langId);
            var target = config.Link?.Myacount ?? "/my-account";
            return Redirect(target);
        }
    }
}

