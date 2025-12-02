using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CMS_2026.Services;
using CMS_2026.Data.Entities;
using CMS_2026.Attributes;
using CMS_2026.ViewModels;
using CMS_2026.Common;
using Root = CMS_2026.Common.Root;

namespace CMS_2026.Pages.Admin
{
    public class SyncModel : BaseAdminPageModel
    {
        public string? Message { get; set; }
        public bool Success { get; set; }
        public List<PP_Compt>? SyncedComponents { get; set; }

        public SyncModel(IDataService dataService, RootService rootService, 
            Microsoft.Extensions.Caching.Memory.IMemoryCache cache, PermissionService permissionService)
            : base(dataService, rootService, cache, permissionService)
        {
        }

        public void OnGet()
        {
            // Display sync page
        }

        public IActionResult OnPost()
        {
            try
            {
                // Delete all existing components
                var allCompts = Db.GetList<PP_Compt>();
                foreach (var compt in allCompts)
                {
                    Db.Delete<PP_Compt>(compt.Id);
                }

                // Run component synchronizer
                var syncedCompts = ComponentSynchronizer.Run(Db);

                Root.ClearCache();
                Root.RefreshConfigs();

                SyncedComponents = syncedCompts;
                Success = true;
                Message = $"Đã đồng bộ thành công {syncedCompts.Count} component!";

                return Page();
            }
            catch (Exception ex)
            {
                Success = false;
                Message = $"Lỗi: {ex.Message}";
                return Page();
            }
        }
    }
}

