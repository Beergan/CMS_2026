using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CMS_2026.Data.Entities;
using CMS_2026.Services;
using CMS_2026.Utils;
using System.Text.RegularExpressions;

namespace CMS_2026.Pages.Admin.User
{
    public class CreateModel : BaseAdminPageModel
    {
        public CreateModel(IDataService dataService, RootService rootService, 
            Microsoft.Extensions.Caching.Memory.IMemoryCache cache, PermissionService permissionService)
            : base(dataService, rootService, cache, permissionService)
        {
        }

        public void OnGet()
        {
        }

        public IActionResult OnPost([FromForm] string UserId, [FromForm] string DisplayName,
            [FromForm] string Email, [FromForm] string Password, [FromForm] bool Enabled)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(UserId) || string.IsNullOrWhiteSpace(DisplayName) 
                    || string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
                {
                    return new JsonResult(new { success = false, message = "Vui lòng điền đầy đủ thông tin!" });
                }

                if (Db.GetList<PP_User>(t => t.UserId == UserId).Any())
                {
                    return new JsonResult(new { success = false, message = "Tên đăng nhập đã tồn tại!" });
                }

                if (!Regex.IsMatch(Email, @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$"))
                {
                    return new JsonResult(new { success = false, message = "Email không hợp lệ!" });
                }

                if (!Regex.IsMatch(Password, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@#$%^&+=!])[A-Za-z\d@#$%^&+=!]{6,15}$"))
                {
                    return new JsonResult(new { success = false, message = "Mật khẩu từ 6-15 ký tự, phải có ít nhất một chữ hoa, chữ thường và một chữ số!" });
                }

                var user = new PP_User
                {
                    UserId = UserId,
                    DisplayName = DisplayName,
                    Email = Email,
                    Password = CryptographyHelper.HashSHA256(Password),
                    Enabled = Enabled
                };

                Db.Insert(user);

                return new JsonResult(new { success = true, message = "Tạo tài khoản thành công!", redirect = "/admin/user" });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }
    }
}

