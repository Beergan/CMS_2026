using Microsoft.AspNetCore.Mvc.RazorPages;
using CMS_2026.Services;

namespace CMS_2026.Pages.Admin.ElFinder
{
    public class IndexModel : PageModel
    {
        public void OnGet()
        {
            // Check authentication
            if (!AuthenticationService.CheckAuthenticatedUser(HttpContext))
            {
                Response.Redirect("/admin/login");
            }
        }
    }
}

