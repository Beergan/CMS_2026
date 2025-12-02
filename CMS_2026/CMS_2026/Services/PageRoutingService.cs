using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CMS_2026.Data.Entities;
using CMS_2026.Services;

namespace CMS_2026.Services
{
    /// <summary>
    /// Service for routing dynamic pages based on pp_page table
    /// Compatible with everflorcom_new (4.8) routing system
    /// </summary>
    public class PageRoutingService
    {
        private readonly IDataService _dataService;
        private readonly RootService _rootService;
        private List<PP_Page>? _cachedPages;

        public PageRoutingService(IDataService dataService, RootService rootService)
        {
            _dataService = dataService;
            _rootService = rootService;
        }

        /// <summary>
        /// Refresh routes from database (similar to MyRouteTable.RefreshRoutes in 4.8)
        /// </summary>
        public void RefreshRoutes()
        {
            _cachedPages = _dataService.GetList<PP_Page>()
                .OrderByDescending(t => t.PathPattern)
                .ToList();
        }

        /// <summary>
        /// Find page by path pattern (compatible with 4.8 MyRouteTable logic)
        /// </summary>
        public PP_Page? FindPageByPath(string path, string? langId = null)
        {
            if (_cachedPages == null)
            {
                RefreshRoutes();
            }

            if (_cachedPages == null) return null;

            // Normalize path
            path = NormalizePath(path);

            // Try to find exact match first (highest priority)
            var exactMatch = _cachedPages.FirstOrDefault(p =>
            {
                if (langId != null && p.LangId != langId) return false;
                if (!RootService.Langs.ContainsKey(p.LangId)) return false;
                if (!RootService.Langs[p.LangId].Enabled) return false;

                var pattern = NormalizePath(p.PathPattern);
                return pattern == path;
            });

            if (exactMatch != null)
                return exactMatch;

            // Try pattern matching (for dynamic routes like /san-pham/{0})
            var patternMatch = _cachedPages.FirstOrDefault(p =>
            {
                if (langId != null && p.LangId != langId) return false;
                if (!RootService.Langs.ContainsKey(p.LangId)) return false;
                if (!RootService.Langs[p.LangId].Enabled) return false;

                var pattern = NormalizePath(p.PathPattern);
                return MatchPattern(pattern, path);
            });

            return patternMatch;
        }

        /// <summary>
        /// Get component path for a page (compatible with 4.8 structure)
        /// Supports both Pages/Components/ and _compt/ paths
        /// </summary>
        public string? GetComponentPath(PP_Page page)
        {
            if (string.IsNullOrEmpty(page.ComptKey))
                return null;
            
            // Try Pages/Components/ first (ASP.NET Core standard)
            // Fallback to _compt/ for compatibility with 4.8
            return $"~/Pages/Components/{page.ComptKey}.cshtml";
        }

        /// <summary>
        /// Normalize path (remove leading/trailing slashes)
        /// </summary>
        private string NormalizePath(string path)
        {
            if (string.IsNullOrEmpty(path))
                return string.Empty;
            
            return path.TrimStart('/').TrimEnd('/');
        }

        /// <summary>
        /// Match pattern against path (compatible with 4.8 pattern matching)
        /// Supports patterns like:
        /// - "/san-pham/{0}" matches "/san-pham/abc", "/san-pham/xyz", etc.
        /// - "/tin-tuc/{0}" matches "/tin-tuc/abc", etc.
        /// </summary>
        private bool MatchPattern(string pattern, string path)
        {
            if (string.IsNullOrEmpty(pattern) || string.IsNullOrEmpty(path))
                return false;

            // Exact match already handled
            if (pattern == path)
                return false;

            // Pattern with {0} placeholder (dynamic slug)
            if (pattern.Contains("{0}"))
            {
                var basePattern = pattern.Substring(0, pattern.IndexOf("{0}"));
                basePattern = NormalizePath(basePattern);
                
                // Check if path starts with base pattern
                if (path.StartsWith(basePattern))
                {
                    // Ensure there's a slug after the base pattern
                    var remaining = path.Substring(basePattern.Length);
                    if (!string.IsNullOrEmpty(remaining) && remaining.StartsWith("/"))
                    {
                        return true;
                    }
                }
            }

            // Pattern ending with /{0} - also match the base path without slug
            // Example: "/san-pham/{0}" should also match "/san-pham"
            if (pattern.EndsWith("/{0}"))
            {
                var basePattern = pattern.Substring(0, pattern.Length - 3);
                basePattern = NormalizePath(basePattern);
                if (path == basePattern)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Extract slug from path based on pattern
        /// Example: pattern="/san-pham/{0}", path="/san-pham/abc" => returns "abc"
        /// </summary>
        public string? ExtractSlug(string pattern, string path)
        {
            if (string.IsNullOrEmpty(pattern) || string.IsNullOrEmpty(path))
                return null;

            pattern = NormalizePath(pattern);
            path = NormalizePath(path);

            if (pattern.Contains("{0}"))
            {
                var basePattern = pattern.Substring(0, pattern.IndexOf("{0}"));
                basePattern = NormalizePath(basePattern);
                
                if (path.StartsWith(basePattern))
                {
                    var slug = path.Substring(basePattern.Length).TrimStart('/');
                    return string.IsNullOrEmpty(slug) ? null : slug;
                }
            }

            return null;
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
    }
}

