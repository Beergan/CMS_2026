using CMS_2026.Data.Entities;
using CMS_2026.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CMS_2026.Pages.Contact
{
    public class SubmitModel : PageModel
    {
        private readonly IDataService _dataService;

        public SubmitModel(IDataService dataService)
        {
            _dataService = dataService;
        }

        public IActionResult OnPost([FromForm] string Name, [FromForm] string Email, [FromForm] string Message, [FromForm] string? Phone)
        {
            var referer = Request.Headers["Referer"].ToString();
            var fallback = string.IsNullOrEmpty(referer) ? "/contact" : referer;

            if (string.IsNullOrWhiteSpace(Name) || string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Message))
            {
                TempData["ContactError"] = "Vui lòng điền đầy đủ thông tin.";
                return Redirect(fallback);
            }

            var contact = new PP_Contact
            {
                Name = Name,
                Email = Email,
                Phone = Phone,
                Message = Message,
                Status = "NEW"
            };

            _dataService.Insert(contact);
            TempData["ContactSuccess"] = "Cảm ơn bạn! Chúng tôi sẽ sớm liên hệ.";
            return Redirect(fallback);
        }
    }
}

