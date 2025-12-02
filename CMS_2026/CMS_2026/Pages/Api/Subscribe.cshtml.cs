using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CMS_2026.Data;
using CMS_2026.Data.Entities;
using CMS_2026.Services;

namespace CMS_2026.Pages.Api
{
    [IgnoreAntiforgeryToken]
    public class SubscribeModel : PageModel
    {
        private readonly IDataService _dataService;

        public SubscribeModel(IDataService dataService)
        {
            _dataService = dataService;
        }

        public IActionResult OnPost([FromForm] string emailRegister)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(emailRegister))
                {
                    return new JsonResult(new { success = false, message = "Email không được để trống." });
                }

                // Validate email format
                if (!System.Text.RegularExpressions.Regex.IsMatch(emailRegister, 
                    @"^[^@\s]+@[^@\s]+\.[^@\s]+$", 
                    System.Text.RegularExpressions.RegexOptions.IgnoreCase))
                {
                    return new JsonResult(new { success = false, message = "Email không hợp lệ." });
                }

                // Check if email already exists
                var existing = _dataService.GetOne<PP_Subscribe>(s => s.Email == emailRegister);
                if (existing != null)
                {
                    return new JsonResult(new { 
                        success = false, 
                        message = "Email này đã được đăng ký trước đó." 
                    });
                }

                // Create new subscription
                var subscribe = new PP_Subscribe
                {
                    Email = emailRegister,
                    SubscribeDate = DateTime.Now,
                    Status = "ACTIVE"
                };

                _dataService.Insert(subscribe);

                return new JsonResult(new
                {
                    success = true,
                    message = "Đăng ký thành công! Cảm ơn bạn đã quan tâm."
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { 
                    success = false, 
                    message = "Có lỗi xảy ra khi đăng ký. Vui lòng thử lại sau." 
                });
            }
        }
    }
}

