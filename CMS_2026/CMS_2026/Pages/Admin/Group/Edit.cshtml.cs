using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CMS_2026.Data.Entities;
using CMS_2026.Services;
using CMS_2026.Models;

namespace CMS_2026.Pages.Admin.Group
{
    public class EditModel : BaseAdminPageModel
    {
        public PP_Roles? Group { get; set; }
        public Dictionary<FeatureModel, System.Tuple<long, string, string>[]> Permissions { get; set; } = new();
        public HashSet<long> SelectedPermissions { get; set; } = new();

        public EditModel(IDataService dataService, RootService rootService, 
            Microsoft.Extensions.Caching.Memory.IMemoryCache cache, PermissionService permissionService)
            : base(dataService, rootService, cache, permissionService)
        {
        }

        public IActionResult OnGet(int? id)
        {
            if (!id.HasValue)
            {
                return Redirect("/admin/group");
            }

            Group = Db.GetOne<PP_Roles>(id.Value);
            if (Group == null)
            {
                return Redirect("/admin/group");
            }

            Permissions = GlobalPermissions.Dictionary;

            // Load selected permissions
            var roleClaims = Db.GetList<PP_RoleClaims>(x => x.RoleId == Group.Id);
            foreach (var claim in roleClaims)
            {
                if (string.IsNullOrEmpty(claim.ClaimType)) continue;
                
                var feature = Permissions.FirstOrDefault(p => p.Key.Name == claim.ClaimType);
                if (feature.Key != null && feature.Value != null)
                {
                    foreach (var perm in feature.Value)
                    {
                        if ((claim.ClaimValue & perm.Item1) == perm.Item1)
                        {
                            SelectedPermissions.Add(perm.Item1);
                        }
                    }
                }
            }

            return Page();
        }

        public IActionResult OnPost([FromForm] int Id, [FromForm] string Name, 
            [FromForm] string? Description, [FromForm] long[]? Permissions)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Name))
                {
                    return new JsonResult(new { success = false, message = "Tên nhóm không được để trống!" });
                }

                var role = Db.GetOne<PP_Roles>(Id);
                if (role == null)
                {
                    return new JsonResult(new { success = false, message = "Không tìm thấy nhóm!" });
                }

                role.Name = Name;
                role.Description = Description;

                Db.Update(role);

                // Update permissions
                var existingClaims = Db.GetList<PP_RoleClaims>(t => t.RoleId == Id);
                foreach (var claim in existingClaims)
                {
                    Db.Delete<PP_RoleClaims>(claim.Id);
                }

                if (Permissions != null && Permissions.Length > 0)
                {
                    var permissionGroups = Permissions.GroupBy(p => 
                        GlobalPermissions.Dictionary.FirstOrDefault(kvp => kvp.Value.Any(v => v.Item1 == p)).Key?.Name ?? "");

                    foreach (var group in permissionGroups)
                    {
                        if (string.IsNullOrEmpty(group.Key)) continue;

                        var totalValue = group.Sum();
                        var roleClaim = new PP_RoleClaims
                        {
                            RoleId = Id,
                            ClaimType = group.Key,
                            ClaimValue = totalValue
                        };
                        Db.Insert(roleClaim);
                    }
                }

                return new JsonResult(new { success = true, message = "Cập nhật thành công!", redirect = "/admin/group" });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }
    }
}

