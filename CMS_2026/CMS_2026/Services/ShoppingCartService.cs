using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using CMS_2026.Data.Entities;
using CMS_2026.Models;
using System.Text.Json;

namespace CMS_2026.Services
{
    public class ShoppingCartService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private const string CartSessionKey = "MyCart";
        private const string ShipFeeKey = "ShipFee";
        private const string ShipProvinceKey = "ShipProvince";
        private const string ShipDistrictKey = "ShipDistrict";
        private const string ShipWardKey = "ShipWard";

        public ShoppingCartService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        private HttpContext Context => _httpContextAccessor.HttpContext ?? throw new InvalidOperationException("HttpContext is null");

        private List<ShoppingCartDetails> CartLines
        {
            get
            {
                var cartJson = Context.Session.GetString(CartSessionKey);
                if (string.IsNullOrEmpty(cartJson))
                {
                    return new List<ShoppingCartDetails>();
                }
                return JsonSerializer.Deserialize<List<ShoppingCartDetails>>(cartJson) ?? new List<ShoppingCartDetails>();
            }
            set
            {
                var cartJson = JsonSerializer.Serialize(value);
                Context.Session.SetString(CartSessionKey, cartJson);
            }
        }

        public decimal ShipFee
        {
            get => decimal.TryParse(Context.Session.GetString(ShipFeeKey), out var fee) ? fee : 0M;
            set => Context.Session.SetString(ShipFeeKey, value.ToString());
        }

        public string ShipProvince
        {
            get => Context.Session.GetString(ShipProvinceKey) ?? string.Empty;
            set => Context.Session.SetString(ShipProvinceKey, value);
        }

        public string ShipDistrict
        {
            get => Context.Session.GetString(ShipDistrictKey) ?? string.Empty;
            set => Context.Session.SetString(ShipDistrictKey, value);
        }

        public string ShipWard
        {
            get => Context.Session.GetString(ShipWardKey) ?? string.Empty;
            set => Context.Session.SetString(ShipWardKey, value);
        }

        public void UpdateOne(PP_Product product, string? variation, int quantity)
        {
            if (product == null) return;

            var lines = CartLines;
            var line = lines.FirstOrDefault(l => l.Product.Id == product.Id && l.Variation == variation);
            var price = product.PromotionEnabled && product.PromotionPrice.HasValue 
                ? product.PromotionPrice.Value 
                : product.Price;

            if (line == null)
            {
                if (quantity > 0)
                {
                    lines.Add(new ShoppingCartDetails
                    {
                        Product = product,
                        Variation = variation,
                        Quantity = quantity,
                        Price = price
                    });
                }
            }
            else if (quantity == 0)
            {
                lines.Remove(line);
            }
            else
            {
                line.Quantity = quantity;
                line.Price = price;
            }

            CartLines = lines;
        }

        public void AddToCart(PP_Product product, string? variation, int quantity)
        {
            if (product == null) return;

            var lines = CartLines;
            var line = lines.FirstOrDefault(l => l.Product.Id == product.Id && l.Variation == variation);
            var price = product.PromotionEnabled && product.PromotionPrice.HasValue 
                ? product.PromotionPrice.Value 
                : product.Price;

            if (line == null)
            {
                lines.Add(new ShoppingCartDetails
                {
                    Product = product,
                    Variation = variation,
                    Quantity = quantity > 0 ? quantity : 1,
                    Price = price
                });
            }
            else
            {
                line.Quantity += quantity > 0 ? quantity : 1;
                line.Price = price;
            }

            CartLines = lines;
        }

        public void RemoveFromCart(PP_Product product, string? variation)
        {
            var lines = CartLines;
            var line = lines.FirstOrDefault(l => l.Product.Id == product.Id && l.Variation == variation);
            if (line != null)
            {
                lines.Remove(line);
                CartLines = lines;
            }
        }

        public List<ShoppingCartDetails> GetCartLines()
        {
            return CartLines;
        }

        public List<OrderLine> GetOrderLines()
        {
            return CartLines.Select(i => new OrderLine
            {
                ID = i.Product.Id,
                Brand = i.Product.Brand,
                Title = i.Product.Title,
                Variation = i.Variation,
                Price = i.Price,
                Quantity = i.Quantity,
                RowTotal = i.Price * i.Quantity,
                Weight = i.Product.Weight,
                PromotionEnabled = i.Product.PromotionEnabled
            }).ToList();
        }

        public int GetQuantity()
        {
            return CartLines.Count;
        }

        public int GetQuantity(PP_Product product)
        {
            var line = CartLines.FirstOrDefault(l => l.Product.Id == product.Id);
            return line?.Quantity ?? 0;
        }

        public decimal GetProductTotal(PP_Product product, string? variation)
        {
            var line = CartLines.FirstOrDefault(l => l.Product.Id == product.Id && l.Variation == variation);
            return line != null ? line.Quantity * line.Price : 0;
        }

        public decimal GetSubTotal()
        {
            return CartLines.Sum(line => line.Price * line.Quantity);
        }

        public int GetTotalWeight()
        {
            return CartLines.Sum(line => line.Product.Weight);
        }

        public decimal GetTotal()
        {
            return GetSubTotal() + ShipFee;
        }

        public void Clear()
        {
            CartLines = new List<ShoppingCartDetails>();
        }

        // Alias methods for compatibility
        public void AddItem(PP_Product product, string? variation, int quantity)
        {
            AddToCart(product, variation, quantity);
        }

        public void RemoveItem(PP_Product product, string? variation)
        {
            RemoveFromCart(product, variation);
        }

        public void UpdateQuantity(PP_Product product, string? variation, int quantity)
        {
            UpdateOne(product, variation, quantity);
        }

        public int GetItemCount()
        {
            return GetQuantity();
        }
    }
}

