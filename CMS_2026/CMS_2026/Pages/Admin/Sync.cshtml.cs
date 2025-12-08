using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
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

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                // Delete all existing components
                var allCompts = await Db.GetListAsync<PP_Compt>();
                foreach (var compt in allCompts)
                {
                    await Db.DeleteAsync<PP_Compt>(compt.Id);
                }

                // Run component synchronizer
                var syncedCompts = await ComponentSynchronizer.RunAsync(Db);

                Root.ClearCache();
                await Root.RefreshConfigsAsync();

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

