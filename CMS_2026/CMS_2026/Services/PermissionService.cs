using System;
using System.Linq;
using System.Threading.Tasks;
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

        public async Task<bool> CheckPermissionAsync<T>(T requiredClaim, int userId) where T : Enum
        {
            var userRole = await _dataService.GetOneAsync<PP_UserRoles>(x => x.UserId == userId);
            if (userRole == null) return false;

            var roleClaims = await _dataService.GetListAsync<PP_RoleClaims>(x => x.RoleId == userRole.RoleId);
            
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

        public bool CheckPermission<T>(T requiredClaim, int userId) where T : Enum
        {
            return CheckPermissionAsync(requiredClaim, userId).GetAwaiter().GetResult();
        }

        public async Task<bool> HasPermissionAsync(int userId, string featureName, long permissionValue)
        {
            var userRole = await _dataService.GetOneAsync<PP_UserRoles>(x => x.UserId == userId);
            if (userRole == null) return false;

            var roleClaim = await _dataService.GetOneAsync<PP_RoleClaims>(x => 
                x.RoleId == userRole.RoleId && x.ClaimType == featureName);

            if (roleClaim == null) return false;

            return (roleClaim.ClaimValue & permissionValue) == permissionValue;
        }

        public bool HasPermission(int userId, string featureName, long permissionValue)
        {
            return HasPermissionAsync(userId, featureName, permissionValue).GetAwaiter().GetResult();
        }
    }
}

