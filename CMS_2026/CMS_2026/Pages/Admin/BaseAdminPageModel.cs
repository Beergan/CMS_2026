using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
using CMS_2026.Data.Entities;
using CMS_2026.Models;
using CMS_2026.Services;
using CMS_2026.Common;
using Root = CMS_2026.Common.Root;

namespace CMS_2026.Pages.Admin
{
    public class BaseAdminPageModel : PageModel
    {
        public IDataService Db { get; protected set; }
        public RootService Root { get; protected set; }
        protected readonly IMemoryCache Cache;
        protected readonly PermissionService PermissionService;

        public BaseAdminPageModel(IDataService dataService, RootService rootService, IMemoryCache cache, PermissionService permissionService)
        {
            Db = dataService;
            Root = rootService;
            Cache = cache;
            PermissionService = permissionService;
        }

        public override void OnPageHandlerExecuting(PageHandlerExecutingContext context)
        {
            if (!AuthenticationService.CheckAuthenticatedUser(HttpContext))
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    context.Result = new StatusCodeResult(401);
                }
                else
                {
                    context.Result = Redirect(Constants.Admin_Login_Url);
                }
                return;
            }

            base.OnPageHandlerExecuting(context);
        }

        public string LangIdCompose
        {
            get
            {
                return Request.Cookies["LangIdCompose"] ?? "vi";
            }
            set
            {
                Response.Cookies.Append("LangIdCompose", value, new Microsoft.AspNetCore.Http.CookieOptions
                {
                    Expires = DateTime.Now.AddYears(1)
                });
            }
        }

        protected string LangIdDisplay
        {
            get
            {
                return Request.Cookies["LangIdDisplay"] ?? "vi";
            }
            set
            {
                Response.Cookies.Append("LangIdDisplay", value, new Microsoft.AspNetCore.Http.CookieOptions
                {
                    Expires = DateTime.Now.AddYears(1)
                });
            }
        }

        public PP_Lang? LangCompose => CMS_2026.Common.Root.Langs.TryGetValue(LangIdCompose, out var lang) ? lang : null;

        /// <summary>
        /// Text translator - Tương đương với MyAdminPage.Text trong everflorcom_new
        /// </summary>
        public Services.AdminTranslator Text => new Services.AdminTranslator(LangIdDisplay);

        public string? UserId => AuthenticationService.GetUserId(HttpContext);
        public int? IdUser => AuthenticationService.GetUserIdInt(HttpContext);
        public string? DisplayName => AuthenticationService.GetDisplayName(HttpContext);

        protected Dictionary<string, string> GetGroupSelector(string langId, string? nodeType = null)
        {
            var query = Db.GetList<PP_Category>(t => t.LangId == langId);

            if (!string.IsNullOrEmpty(nodeType))
            {
                query = query.Where(t => t.NodeType == nodeType).ToList();
            }

            var items = query
                .OrderBy(o => o.CategoryPath)
                .ToList();

            var options = new Dictionary<string, string> 
            { 
                { "0", "Không có chuyên mục" } 
            };

            var matrix = new List<int>[items.Count];

            for (int i = 0; i < items.Count; i++)
            {
                matrix[i] = Enumerable
                    .Range(0, items[i].CategoryLevel)
                    .Select(t => 0)
                    .ToList();

                int col = items[i].CategoryLevel - 1;
                matrix[i][col] = 1;

                for (var j = i - 1; j >= 0; j--)
                {
                    if (matrix[j].Count - 1 < col)
                    {
                        break;
                    }

                    if (matrix[j][col] == 1)
                    {
                        matrix[j][col] = 2;
                        break;
                    }

                    matrix[j][col] = 3;
                }
            }

            for (int i = 0; i < items.Count; i++)
            {
                string str = string.Empty;

                for (int j = 0; j < items[i].CategoryLevel; j++)
                {
                    switch (matrix[i][j])
                    {
                        case 0:
                            str += "\xA0\xA0\xA0\xA0\xA0\xA0\xA0\xA0\xA0\xA0";
                            break;
                        case 1:
                            str += "+───\xA0";
                            break;
                        case 2:
                            str += "+───\xA0";
                            break;
                        case 3:
                            str += "│\xA0\xA0\xA0\xA0\xA0\xA0\xA0\xA0";
                            break;
                    }
                }

                options.Add(items[i].Id.ToString(), str + $"{items[i].Title} (/{items[i].CategoryPath})");
            }

            return options;
        }
    }
}

