using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CMS_2026.Data.Entities;
using CMS_2026.Services;
using CMS_2026.Models;
using Newtonsoft.Json.Linq;

namespace CMS_2026.Pages.Admin.Permission
{
    public class IndexModel : BaseAdminPageModel
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public Dictionary<FeatureModel, System.Tuple<long, string, string>[]> Permissions { get; set; } = new();
        public HashSet<long> SelectedPermissions { get; set; } = new();

        public IndexModel(IDataService dataService, RootService rootService, 
            Microsoft.Extensions.Caching.Memory.IMemoryCache cache, PermissionService permissionService)
            : base(dataService, rootService, cache, permissionService)
        {
        }

        public IActionResult OnGet(int? roleid)
        {
            if (!roleid.HasValue)
            {
                return Redirect("/admin/group");
            }

            RoleId = roleid.Value;
            Permissions = GlobalPermissions.Dictionary;

            var role = Db.GetOne<PP_Roles>(RoleId);
            RoleName = role?.RoleName ?? "Unknown";

            var roleClaims = Db.GetList<PP_RoleClaims>(x => x.RoleId == RoleId)
                .Where(x => !string.IsNullOrEmpty(x.ClaimType))
                .ToDictionary(x => x.ClaimType!, x => x.ClaimValue);

            foreach (var feature in Permissions)
            {
                if (string.IsNullOrEmpty(feature.Key.Name)) continue;
                
                if (roleClaims.ContainsKey(feature.Key.Name))
                {
                    var claimValue = roleClaims[feature.Key.Name];
                    if (feature.Value != null)
                    {
                        foreach (var permission in feature.Value)
                        {
                            if ((claimValue & permission.Item1) == permission.Item1)
                            {
                                SelectedPermissions.Add(permission.Item1);
                            }
                        }
                    }
                }
            }

            return Page();
        }

        public IActionResult OnPost([FromForm] int roleid, [FromForm] string? data)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(data))
                {
                    return new JsonResult(new { success = false, message = "Dữ liệu không hợp lệ!" });
                }

                var roleClaims = Db.GetList<PP_RoleClaims>(x => x.RoleId == roleid);
                foreach (var claim in roleClaims)
                {
                    Db.Delete<PP_RoleClaims>(claim.Id);
                }

                var obj = JObject.Parse(data);
                var groups = obj["groups"]?.ToArray() ?? new JToken[0];

                foreach (var group in groups)
                {
                    var groupId = group["groupId"]?.ToString();
                    var options = group["options"]?.ToArray() ?? new JToken[0];
                    var totalValue = options.Where(o => o["checked"]?.Value<bool>() == true)
                        .Sum(o => o["value"]?.Value<long>() ?? 0);

                    if (totalValue > 0 && !string.IsNullOrEmpty(groupId))
                    {
                        var roleClaim = new PP_RoleClaims
                        {
                            RoleId = roleid,
                            ClaimType = groupId,
                            ClaimValue = totalValue
                        };
                        Db.Insert(roleClaim);
                    }
                }

                return new JsonResult(new { success = true, message = "Cập nhật thành công!" });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }
    }
}

