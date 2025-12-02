using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CMS_2026.Data.Entities;
using CMS_2026.Services;
using CMS_2026.Utils;
using System.Text.RegularExpressions;

namespace CMS_2026.Pages.Admin.User
{
    public class EditModel : BaseAdminPageModel
    {
        public new PP_User? User { get; set; }

        public EditModel(IDataService dataService, RootService rootService, 
            Microsoft.Extensions.Caching.Memory.IMemoryCache cache, PermissionService permissionService)
            : base(dataService, rootService, cache, permissionService)
        {
        }

        public IActionResult OnGet(int? id)
        {
            if (!id.HasValue)
            {
                return Redirect("/admin/user");
            }

            User = Db.GetOne<PP_User>(id.Value);
            if (User == null)
            {
                return Redirect("/admin/user");
            }

            return Page();
        }

        public IActionResult OnPost([FromForm] int Id, [FromForm] string DisplayName,
            [FromForm] string Email, [FromForm] string? Password, [FromForm] bool Enabled)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(DisplayName) || string.IsNullOrWhiteSpace(Email))
                {
                    return new JsonResult(new { success = false, message = "Vui lòng điền đầy đủ thông tin!" });
                }

                var user = Db.GetOne<PP_User>(Id);
                if (user == null)
                {
                    return new JsonResult(new { success = false, message = "Không tìm thấy tài khoản!" });
                }

                if (!Regex.IsMatch(Email, @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$"))
                {
                    return new JsonResult(new { success = false, message = "Email không hợp lệ!" });
                }

                user.DisplayName = DisplayName;
                user.Email = Email;
                user.Enabled = Enabled;

                if (!string.IsNullOrWhiteSpace(Password))
                {
                    if (!Regex.IsMatch(Password, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@#$%^&+=!])[A-Za-z\d@#$%^&+=!]{6,15}$"))
                    {
                        return new JsonResult(new { success = false, message = "Mật khẩu từ 6-15 ký tự, phải có ít nhất một chữ hoa, chữ thường và một chữ số!" });
                    }
                    user.Password = CryptographyHelper.HashSHA256(Password);
                }

                Db.Update(user);

                return new JsonResult(new { success = true, message = "Cập nhật thành công!", redirect = "/admin/user" });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }
    }
}

