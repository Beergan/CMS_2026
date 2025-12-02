using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using CMS_2026.Models;

namespace CMS_2026.Services
{
    public class GHTKService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiUrl;
        private readonly string _token;

        public GHTKService(HttpClient httpClient, GHTKConfig? config)
        {
            _httpClient = httpClient;
            _apiUrl = config?.GhtkApi ?? "https://services.giaohangtietkiem.vn";
            _token = config?.GhtkToken ?? string.Empty;
            
            if (!string.IsNullOrEmpty(_token))
            {
                _httpClient.DefaultRequestHeaders.Add("Token", _token);
            }
        }

        public async Task<GHTKFeeResponse?> CalculateFee(GHTKFeeRequest request)
        {
            try
            {
                var json = JsonSerializer.Serialize(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                
                var response = await _httpClient.PostAsync($"{_apiUrl}/services/shipment/fee", content);
                var responseContent = await response.Content.ReadAsStringAsync();
                
                return JsonSerializer.Deserialize<GHTKFeeResponse>(responseContent);
            }
            catch
            {
                return null;
            }
        }

        public async Task<GHTKOrderResponse?> CreateOrder(GHTKOrderRequest request)
        {
            try
            {
                var json = JsonSerializer.Serialize(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                
                var response = await _httpClient.PostAsync($"{_apiUrl}/services/shipment/order", content);
                var responseContent = await response.Content.ReadAsStringAsync();
                
                return JsonSerializer.Deserialize<GHTKOrderResponse>(responseContent);
            }
            catch
            {
                return null;
            }
        }
    }

    public class GHTKFeeRequest
    {
        public string PickAddress { get; set; } = string.Empty;
        public string PickProvince { get; set; } = string.Empty;
        public string PickDistrict { get; set; } = string.Empty;
        public string PickWard { get; set; } = string.Empty;
        public string Province { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public string Ward { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public int Weight { get; set; }
        public decimal Value { get; set; }
        public string Transport { get; set; } = "road";
    }

    public class GHTKFeeResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public GHTKFee? Fee { get; set; }
    }

    public class GHTKFee
    {
        public string? Name { get; set; }
        public decimal Fee { get; set; }
        public int InsuranceFee { get; set; }
    }

    public class GHTKOrderRequest
    {
        public List<GHTKProduct> Products { get; set; } = new();
        public GHTKOrder Order { get; set; } = new();
    }

    public class GHTKProduct
    {
        public string Name { get; set; } = string.Empty;
        public double Weight { get; set; }
        public int Quantity { get; set; }
        public string ProductCode { get; set; } = string.Empty;
        public int Price { get; set; }
    }

    public class GHTKOrder
    {
        public string Id { get; set; } = string.Empty;
        public string PickName { get; set; } = string.Empty;
        public string PickAddress { get; set; } = string.Empty;
        public string PickProvince { get; set; } = string.Empty;
        public string PickDistrict { get; set; } = string.Empty;
        public string PickWard { get; set; } = string.Empty;
        public string PickTel { get; set; } = string.Empty;
        public string Tel { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Province { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public string Ward { get; set; } = string.Empty;
        public string Note { get; set; } = string.Empty;
        public int PickMoney { get; set; }
        public int Value { get; set; }
        public string Transport { get; set; } = "road";
    }

    public class GHTKOrderResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public GHTKOrderResult? Order { get; set; }
    }

    public class GHTKOrderResult
    {
        public string? PartnerId { get; set; }
        public string? Label { get; set; }
        public decimal Fee { get; set; }
    }
}

