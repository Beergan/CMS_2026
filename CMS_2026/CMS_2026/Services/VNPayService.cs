using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace CMS_2026.Services
{
    public class VNPayService
    {
        private readonly string _vnp_TmnCode;
        private readonly string _vnp_HashSecret;
        private readonly string _vnp_Url;
        private readonly Dictionary<string, string> _requestData = new();
        private readonly Dictionary<string, string> _responseData = new();

        public VNPayService(string tmnCode, string hashSecret, string url)
        {
            _vnp_TmnCode = tmnCode;
            _vnp_HashSecret = hashSecret;
            _vnp_Url = url;
        }

        public void AddRequestData(string key, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                _requestData[key] = value;
            }
        }

        public void AddResponseData(string key, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                _responseData[key] = value;
            }
        }

        public string GetResponseData(string key)
        {
            return _responseData.TryGetValue(key, out var value) ? value : string.Empty;
        }

        public string CreatePaymentUrl(string orderCode, decimal totalAmount, string returnUrl, string? ipAddress, string? bankCode = null)
        {
            try
            {
                long amountInCents = (long)(totalAmount * 100);

                AddRequestData("vnp_Amount", amountInCents.ToString());
                AddRequestData("vnp_Command", "pay");
                AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
                AddRequestData("vnp_CurrCode", "VND");
                AddRequestData("vnp_IpAddr", ipAddress ?? "127.0.0.1");
                AddRequestData("vnp_Locale", "vn");
                AddRequestData("vnp_OrderInfo", "Thanh toán đơn hàng " + orderCode);
                AddRequestData("vnp_OrderType", "other");
                AddRequestData("vnp_ReturnUrl", returnUrl);
                AddRequestData("vnp_TmnCode", _vnp_TmnCode);
                AddRequestData("vnp_TxnRef", orderCode);
                AddRequestData("vnp_Version", "2.1.0");

                if (!string.IsNullOrEmpty(bankCode))
                {
                    AddRequestData("vnp_BankCode", bankCode);
                }

                return CreateRequestUrl(_vnp_Url, _vnp_HashSecret);
            }
            catch (Exception ex)
            {
                throw new Exception("Error creating payment URL", ex);
            }
        }

        public bool ProcessPaymentResponse(IQueryCollection queryString)
        {
            foreach (var key in queryString.Keys)
            {
                AddResponseData(key, queryString[key].ToString());
            }

            string vnp_SecureHash = GetResponseData("vnp_SecureHash");
            if (ValidateSignature(vnp_SecureHash))
            {
                string vnp_ResponseCode = GetResponseData("vnp_ResponseCode");
                return vnp_ResponseCode == "00";
            }

            return false;
        }

        public bool ValidateSignature(string secureHash)
        {
            var inputData = GetResponseData();
            var computedHash = HmacSHA512(_vnp_HashSecret, inputData);
            return secureHash.Equals(computedHash, StringComparison.OrdinalIgnoreCase);
        }

        private string CreateRequestUrl(string baseUrl, string vnp_HashSecret)
        {
            var sortedData = _requestData.OrderBy(x => x.Key, new VnPayCompare()).ToList();
            var data = new StringBuilder();
            
            foreach (var kv in sortedData)
            {
                if (!string.IsNullOrEmpty(kv.Value))
                {
                    data.Append(WebUtility.UrlEncode(kv.Key) + "=" + WebUtility.UrlEncode(kv.Value) + "&");
                }
            }

            string queryString = data.ToString();
            baseUrl += "?" + queryString;
            
            string signData = queryString;
            if (signData.Length > 0)
            {
                signData = signData.Remove(signData.Length - 1, 1);
            }

            string vnp_SecureHash = HmacSHA512(vnp_HashSecret, signData);
            baseUrl += "vnp_SecureHash=" + vnp_SecureHash;

            return baseUrl;
        }

        private string GetResponseData()
        {
            var data = new StringBuilder();
            var sortedData = _responseData
                .Where(kv => kv.Key != "vnp_SecureHash" && kv.Key != "vnp_SecureHashType")
                .OrderBy(x => x.Key, new VnPayCompare())
                .ToList();

            foreach (var kv in sortedData)
            {
                if (!string.IsNullOrEmpty(kv.Value))
                {
                    data.Append(WebUtility.UrlEncode(kv.Key) + "=" + WebUtility.UrlEncode(kv.Value) + "&");
                }
            }

            if (data.Length > 0)
            {
                data.Remove(data.Length - 1, 1);
            }

            return data.ToString();
        }

        private static string HmacSHA512(string key, string inputData)
        {
            var hash = new StringBuilder();
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            byte[] inputBytes = Encoding.UTF8.GetBytes(inputData);
            
            using (var hmac = new HMACSHA512(keyBytes))
            {
                byte[] hashValue = hmac.ComputeHash(inputBytes);
                foreach (var theByte in hashValue)
                {
                    hash.Append(theByte.ToString("x2"));
                }
            }

            return hash.ToString();
        }

        private class VnPayCompare : IComparer<string>
        {
            public int Compare(string? x, string? y)
            {
                if (x == y) return 0;
                if (x == null) return -1;
                if (y == null) return 1;
                var vnpCompare = CompareInfo.GetCompareInfo("en-US");
                return vnpCompare.Compare(x, y, CompareOptions.Ordinal);
            }
        }
    }
}

