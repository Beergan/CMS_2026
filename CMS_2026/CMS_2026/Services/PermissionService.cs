using System;
using System.Linq;
using CMS_2026.Data.Entities;
using CMS_2026.Models;
using CMS_2026.Services;

namespace CMS_2026.Services
{
    public class PermissionService
    {
        private readonly IDataService _dataService;

        public PermissionService(IDataService dataService)
        {
            _dataService = dataService;
        }

        public bool CheckPermission<T>(T requiredClaim, int userId) where T : Enum
        {
            var userRole = _dataService.GetOne<PP_UserRoles>(x => x.UserId == userId);
            if (userRole == null) return false;

            var roleClaims = _dataService.GetList<PP_RoleClaims>(x => x.RoleId == userRole.RoleId);
            
            var featureAttrb = typeof(T).GetCustomAttributes(typeof(Attributes.FeatureAttribute), false)
                .FirstOrDefault() as Attributes.FeatureAttribute;
            
            if (featureAttrb == null) return false;

            long requiredPermission = Convert.ToInt64(Math.Pow(2, Convert.ToInt64(requiredClaim)));
            long availablePermission = roleClaims
                .Where(x => x.ClaimType == featureAttrb.Name)
                .Select(x => x.ClaimValue)
                .FirstOrDefault();

            if (availablePermission == 0) return false;

            return (availablePermission & requiredPermission) == requiredPermission;
        }

        public bool HasPermission(int userId, string featureName, long permissionValue)
        {
            var userRole = _dataService.GetOne<PP_UserRoles>(x => x.UserId == userId);
            if (userRole == null) return false;

            var roleClaim = _dataService.GetOne<PP_RoleClaims>(x => 
                x.RoleId == userRole.RoleId && x.ClaimType == featureName);

            if (roleClaim == null) return false;

            return (roleClaim.ClaimValue & permissionValue) == permissionValue;
        }
    }
}

