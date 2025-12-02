using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CMS_2026.Services;

namespace CMS_2026.Pages.Admin
{
    public class LogoutModel : PageModel
    {
        public IActionResult OnGet()
        {
            AuthenticationService.Logout(HttpContext);
            return Redirect("/admin/login");
        }
    }
}

