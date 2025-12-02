using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CMS_2026.Data.Entities;
using CMS_2026.Services;
using CMS_2026.Utils;

namespace CMS_2026.Pages.Admin
{
    public class ChangePwdModel : BaseAdminPageModel
    {
        [BindProperty]
        public string? CurrentPassword { get; set; }

        [BindProperty]
        public string? NewPassword { get; set; }

        [BindProperty]
        public string? ConfirmationPassword { get; set; }

        public string? ErrorMessage { get; set; }

        public ChangePwdModel(IDataService dataService, RootService rootService, 
            Microsoft.Extensions.Caching.Memory.IMemoryCache cache, PermissionService permissionService)
            : base(dataService, rootService, cache, permissionService)
        {
        }

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            if (string.IsNullOrWhiteSpace(CurrentPassword) || string.IsNullOrWhiteSpace(NewPassword) 
                || string.IsNullOrWhiteSpace(ConfirmationPassword))
            {
                ErrorMessage = "Vui lòng điền đầy đủ thông tin!";
                return Page();
            }

            if (CurrentPassword.Length < 6 || NewPassword.Length < 6)
            {
                ErrorMessage = "Mật khẩu phải gồm ít nhất 6 ký tự!";
                return Page();
            }

            if (NewPassword != ConfirmationPassword)
            {
                ErrorMessage = "Xác nhận mật khẩu không khớp!";
                return Page();
            }

            if (!IdUser.HasValue)
            {
                ErrorMessage = "Không tìm thấy thông tin người dùng!";
                return Page();
            }

            try
            {
                var currentPasswordHash = CryptographyHelper.HashSHA256(CurrentPassword);
                var checkUser = Db.GetOne<PP_User>(t => t.Id == IdUser.Value && t.Password == currentPasswordHash);

                if (checkUser == null)
                {
                    ErrorMessage = "Mật khẩu hiện tại không đúng!";
                    return Page();
                }

                checkUser.Password = CryptographyHelper.HashSHA256(NewPassword);
                Db.Update(checkUser);

                AuthenticationService.Logout(HttpContext);
                return Redirect("/admin/login");
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                return Page();
            }
        }
    }
}

