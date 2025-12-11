using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using CMS_2026.Data.Entities;
using CMS_2026.Services;

namespace CMS_2026.Middleware
{
    public class DynamicPageMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public DynamicPageMiddleware(RequestDelegate next, IServiceScopeFactory serviceScopeFactory)
        {
            _next = next;
            _serviceScopeFactory = serviceScopeFactory;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.Value ?? string.Empty;

            if (path.StartsWith("/admin") ||
                path.StartsWith("/Error") ||
                path.StartsWith("/_") ||
                path.Contains(".") && !path.EndsWith(".cshtml"))
            {
                await _next(context);
                return;
            }

            using var scope = _serviceScopeFactory.CreateScope();
            var routingService = scope.ServiceProvider.GetRequiredService<PageRoutingService>();

            var langId = context.Request.Cookies["LangId"]
                ?? context.Request.Query["lang"].FirstOrDefault()
                ?? "vi";
            var page = routingService.FindPageByPath(path, langId);

            if (page != null)
            {
                if (!RootService.Langs.ContainsKey(page.LangId) ||
                    !RootService.Langs[page.LangId].Enabled)
                {
                    await _next(context);
                    return;
                }

                context.Items["OriginalPath"] = path;

                context.Items["PageId"] = page.Id;
                context.Items["LangId"] = page.LangId;
                context.Items["ComptKey"] = page.ComptKey;
                context.Items["PP_Page"] = page;

                string? nodeSlug = null;

                if (page.PathPattern.Contains("{0}"))
                {
                    var slug = routingService.ExtractSlug(page.PathPattern, path);
                    if (!string.IsNullOrEmpty(slug))
                    {
                        nodeSlug = slug;
                    }
                    else
                    {
                        nodeSlug = path.TrimStart('/').Split('?').FirstOrDefault();
                    }
                }
                else
                {
                    nodeSlug = path.TrimStart('/').Split('?').FirstOrDefault();
                }

                if (!string.IsNullOrEmpty(nodeSlug))
                {
                    context.Items["NodeSlug"] = nodeSlug;
                }

                context.Request.Path = new PathString("/DynamicPage");
            }

            await _next(context);
        }
    }
}

