using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using CMS_2026.Utils;

namespace CMS_2026.Services
{
    public static class AuthenticationService
    {
        private static bool CheckCookieLogin(string cookie, string ip, string domain)
        {
            try
            {
                string check = CryptographyHelper.Decrypt(cookie);
                return check == ("yes" + ip + domain);
            }
            catch
            {
                return false;
            }
        }

        public static bool CheckAuthenticatedUser(HttpContext context)
        {
            if (!context.Request.Cookies.TryGetValue("check", out var cookieValue))
                return false;

            var loginCookie = cookieValue;
            if (string.IsNullOrEmpty(loginCookie))
                return false;

            var ip = context.Request.Headers["CF-Connecting-IP"].FirstOrDefault() 
                  ?? context.Request.Headers["X-Forwarded-For"].FirstOrDefault()
                  ?? context.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1";
            
            var domain = $"{context.Request.Scheme}://{context.Request.Host}";

            return CheckCookieLogin(loginCookie, ip, domain);
        }

        public static void WriteAuthenCookie(HttpContext context, string userId, string password, string displayName, int id)
        {
            var ip = context.Request.Headers["CF-Connecting-IP"].FirstOrDefault() 
                  ?? context.Request.Headers["X-Forwarded-For"].FirstOrDefault()
                  ?? context.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1";
            
            var domain = $"{context.Request.Scheme}://{context.Request.Host}";

            var loginCookie = CryptographyHelper.Encrypt("yes" + ip + domain);

            var cookieOptions = new CookieOptions
            {
                Expires = DateTime.Now.AddDays(1),
                HttpOnly = true,
                Secure = context.Request.IsHttps,
                SameSite = SameSiteMode.Lax
            };

            context.Response.Cookies.Append("check", loginCookie, cookieOptions);
            context.Response.Cookies.Append("check_user", userId, cookieOptions);
            context.Response.Cookies.Append("check_iduser", id.ToString(), cookieOptions);
            context.Response.Cookies.Append("check_displayName", displayName.EncodeBase64(), cookieOptions);
        }

        public static void Logout(HttpContext context)
        {
            context.Response.Cookies.Delete("check");
            context.Response.Cookies.Delete("check_user");
            context.Response.Cookies.Delete("check_iduser");
            context.Response.Cookies.Delete("check_displayName");
        }

        public static string? GetUserId(HttpContext context)
        {
            return context.Request.Cookies["check_user"];
        }

        public static int? GetUserIdInt(HttpContext context)
        {
            var idStr = context.Request.Cookies["check_iduser"];
            if (int.TryParse(idStr, out var id))
                return id;
            return null;
        }

        public static string? GetDisplayName(HttpContext context)
        {
            var displayName = context.Request.Cookies["check_displayName"];
            if (string.IsNullOrEmpty(displayName))
                return null;

            if (displayName.IsBase64String())
                return displayName.DecodeBase64();

            return displayName;
        }
    }
}

