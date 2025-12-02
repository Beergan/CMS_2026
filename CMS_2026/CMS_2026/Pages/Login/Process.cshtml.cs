using System;
using CMS_2026.Data.Entities;
using CMS_2026.Services;
using CMS_2026.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CMS_2026.Pages.Login
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

        public IActionResult OnPost([FromForm] string Email, [FromForm] string Password, [FromForm] string? ReturnUrl)
        {
            var referer = Request.Headers["Referer"].ToString();
            var fallback = string.IsNullOrEmpty(referer) ? "/login" : referer;

            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                TempData["LoginError"] = "Vui lòng nhập email và mật khẩu.";
                return Redirect(fallback);
            }

            var account = _dataService.GetOne<PP_Register>(x => x.Email == Email);
            if (account == null)
            {
                TempData["LoginError"] = "Tài khoản không tồn tại.";
                return Redirect(fallback);
            }

            var hash = CryptographyHelper.HashSHA256(Password);
            if (!string.Equals(account.PASSWORD, hash, StringComparison.OrdinalIgnoreCase))
            {
                TempData["LoginError"] = "Sai mật khẩu.";
                return Redirect(fallback);
            }

            HttpContext.Session.SignInCustomer(account);
            TempData["LoginSuccess"] = "Đăng nhập thành công!";

            if (!string.IsNullOrEmpty(ReturnUrl))
            {
                return Redirect(ReturnUrl);
            }

            var langId = Request.Cookies["LangId"] ?? "vi";
            var config = _rootService.GetConfig(langId);
            var target = config.Link?.Myacount ?? "/my-account";
            return Redirect(target);
        }
    }
}

