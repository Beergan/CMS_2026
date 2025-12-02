using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CMS_2026.Common;

namespace CMS_2026.Pages.Admin
{
    public class SwitchLangModel : PageModel
    {
        public IActionResult OnGet()
        {
            string langId = "vi";
            string redirect = Constants.Admin_Url;

            if (Request.Query.ContainsKey("vi"))
            {
                redirect = Request.Query["vi"].ToString();
            }
            else if (Request.Query.ContainsKey("en"))
            {
                langId = "en";
                redirect = Request.Query["en"].ToString();
            }

            // Set language cookie
            Response.Cookies.Append("LangIdDisplay", langId, new Microsoft.AspNetCore.Http.CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddYears(1),
                HttpOnly = true
            });

            return Redirect(redirect);
        }
    }
}

