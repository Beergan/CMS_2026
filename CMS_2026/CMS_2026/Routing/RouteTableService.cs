using System.Collections.Generic;
using System.Linq;
using CMS_2026.Data.Entities;
using CMS_2026.Services;
using CMS_2026.Common;

namespace CMS_2026.Routing
{
    /// <summary>
    /// Service for managing dynamic routes (similar to MyRouteTable in everflorcom_new)
    /// Provides methods to refresh and manage routes from pp_page table
    /// </summary>
    public class RouteTableService
    {
        private readonly IDataService _dataService;
        private readonly RootService _rootService;
        private List<PP_Page>? _cachedPages;

        public RouteTableService(IDataService dataService, RootService rootService)
        {
            _dataService = dataService;
            _rootService = rootService;
        }

        /// <summary>
        /// Refresh routes from database (compatible with MyRouteTable.RefreshRoutes in 4.8)
        /// </summary>
        public void RefreshRoutes(List<PP_Page>? pages = null)
        {
            _cachedPages = pages ?? _dataService.GetList<PP_Page>()
                .OrderByDescending(t => t.PathPattern)
                .ToList();
        }

        /// <summary>
        /// Get all enabled pages for routing
        /// Filters by enabled languages (similar to MyRouteTable logic)
        /// </summary>
        public List<PP_Page> GetEnabledPages()
        {
            if (_cachedPages == null)
            {
                RefreshRoutes();
            }

            if (_cachedPages == null)
                return new List<PP_Page>();

            // Filter by enabled languages (same logic as MyRouteTable)
            return _cachedPages
                .Where(p => RootService.Langs.ContainsKey(p.LangId) && 
                           RootService.Langs[p.LangId].Enabled)
                .ToList();
        }

        /// <summary>
        /// Get all pages (for admin or debugging)
        /// </summary>
        public List<PP_Page> GetAllPages()
        {
            if (_cachedPages == null)
            {
                RefreshRoutes();
            }
            return _cachedPages ?? new List<PP_Page>();
        }

        /// <summary>
        /// Clear cache to force refresh on next access
        /// </summary>
        public void ClearCache()
        {
            _cachedPages = null;
        }
    }
}

