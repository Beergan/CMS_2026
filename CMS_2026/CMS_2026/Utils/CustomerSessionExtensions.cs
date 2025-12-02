using CMS_2026.Data.Entities;
using Microsoft.AspNetCore.Http;

namespace CMS_2026.Utils
{
    public static class CustomerSessionExtensions
    {
        private const string CustomerIdKey = "CustomerId";
        private const string CustomerNameKey = "CustomerName";
        private const string CustomerEmailKey = "CustomerEmail";

        public static void SignInCustomer(this ISession session, PP_Register customer)
        {
            session.SetInt32(CustomerIdKey, customer.Id);
            session.SetString(CustomerNameKey, customer.Name ?? string.Empty);
            session.SetString(CustomerEmailKey, customer.Email ?? string.Empty);
        }

        public static void SignOutCustomer(this ISession session)
        {
            session.Remove(CustomerIdKey);
            session.Remove(CustomerNameKey);
            session.Remove(CustomerEmailKey);
        }

        public static int? GetCustomerId(this ISession session)
        {
            return session.GetInt32(CustomerIdKey);
        }

        public static string? GetCustomerName(this ISession session)
        {
            return session.GetString(CustomerNameKey);
        }

        public static string? GetCustomerEmail(this ISession session)
        {
            return session.GetString(CustomerEmailKey);
        }
    }
}

