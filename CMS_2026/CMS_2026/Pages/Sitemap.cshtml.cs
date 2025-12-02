using CMS_2026.Data;
using CMS_2026.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CMS_2026.Pages
{
    public class SitemapModel : PageModel
    {
        private readonly IDataService _dataService;
        private readonly RootService _rootService;

        public SitemapModel(IDataService dataService, RootService rootService)
        {
            _dataService = dataService;
            _rootService = rootService;
        }

        public List<SitemapUrl> Urls { get; set; } = new();

        public void OnGet()
        {
            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            var now = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss+00:00");

            // Homepage
            Urls.Add(new SitemapUrl
            {
                Location = baseUrl,
                LastMod = now,
                Priority = "1.00"
            });

            // Get all languages
            var languages = _dataService.GetList<CMS_2026.Data.Entities.PP_Lang>(l => l.Status == "ACTIVE");

            foreach (var lang in languages)
            {
                var config = _rootService.GetConfig(lang.LangId);

                // Pages
                var pages = _dataService.GetList<CMS_2026.Data.Entities.PP_Page>(
                    p => p.LangId == lang.LangId && p.Status == "ACTIVE");

                foreach (var page in pages)
                {
                    Urls.Add(new SitemapUrl
                    {
                        Location = $"{baseUrl}/{lang.LangId}/{page.NodePath}",
                        LastMod = page.UpdatedTime?.ToString("yyyy-MM-ddTHH:mm:ss+00:00") ?? now,
                        Priority = "0.80"
                    });
                }

                // Products
                var products = _dataService.GetList<CMS_2026.Data.Entities.PP_Product>(
                    p => p.LangId == lang.LangId && p.Status == "ACTIVE");

                foreach (var product in products)
                {
                    Urls.Add(new SitemapUrl
                    {
                        Location = $"{baseUrl}/{lang.LangId}/san-pham/{product.NodePath}",
                        LastMod = product.UpdatedTime?.ToString("yyyy-MM-ddTHH:mm:ss+00:00") ?? now,
                        Priority = "0.80"
                    });
                }

                // Posts/Blogs
                var posts = _dataService.GetList<CMS_2026.Data.Entities.PP_Node>(
                    p => p.LangId == lang.LangId && p.NodeType == "post" && p.Status == "ACTIVE");

                foreach (var post in posts)
                {
                    Urls.Add(new SitemapUrl
                    {
                        Location = $"{baseUrl}/{lang.LangId}/tin-tuc/{post.NodePath}",
                        LastMod = post.UpdatedTime?.ToString("yyyy-MM-ddTHH:mm:ss+00:00") ?? now,
                        Priority = "0.80"
                    });
                }
            }
        }
    }

    public class SitemapUrl
    {
        public string Location { get; set; } = string.Empty;
        public string LastMod { get; set; } = string.Empty;
        public string Priority { get; set; } = "0.50";
    }
}

