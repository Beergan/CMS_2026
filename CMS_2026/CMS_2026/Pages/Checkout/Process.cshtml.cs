using System;
using System.Linq;
using System.Text.Json;
using CMS_2026.Data.Entities;
using CMS_2026.Models;
using CMS_2026.Services;
using CMS_2026.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CMS_2026.Pages.Checkout
{
    public class ProcessModel : PageModel
    {
        private readonly IDataService _dataService;
        private readonly RootService _rootService;
        private readonly ShoppingCartService _cartService;
        private readonly VietQRService _vietQrService;

        public ProcessModel(IDataService dataService, RootService rootService, ShoppingCartService cartService, VietQRService vietQrService)
        {
            _dataService = dataService;
            _rootService = rootService;
            _cartService = cartService;
            _vietQrService = vietQrService;
        }

        public IActionResult OnPost([FromForm] string CustomerName, [FromForm] string Email, [FromForm] string Phone,
            [FromForm] string Address, [FromForm] string PaymentMethod, [FromForm] string? Note)
        {
            var referer = Request.Headers["Referer"].ToString();
            var fallback = string.IsNullOrEmpty(referer) ? "/checkout" : referer;

            if (string.IsNullOrWhiteSpace(CustomerName) || string.IsNullOrWhiteSpace(Email) ||
                string.IsNullOrWhiteSpace(Phone) || string.IsNullOrWhiteSpace(Address))
            {
                TempData["CheckoutError"] = "Vui lòng điền đầy đủ thông tin khách hàng.";
                return Redirect(fallback);
            }

            var orderLines = _cartService.GetOrderLines();
            if (!orderLines.Any())
            {
                TempData["CheckoutError"] = "Giỏ hàng của bạn đang trống.";
                return Redirect(fallback);
            }

            var subTotal = _cartService.GetSubTotal();
            var shipFee = _cartService.ShipFee;
            var total = subTotal + shipFee;
            var orderCode = $"ORD{DateTime.UtcNow:yyyyMMddHHmmssfff}";

            var order = new PP_Order
            {
                OrderCode = orderCode,
                Name = CustomerName,
                Email = Email,
                Phone = Phone,
                DeliveryAddress = Address,
                OrderStatus = "NEW",
                PayMethod = PaymentMethod,
                SubTotalAmount = subTotal,
                ShipFee = shipFee,
                TotalAmount = total,
                Note = Note,
                JsonData = JsonSerializer.Serialize(orderLines),
                IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString()
            };

            _dataService.Insert(order);

            var serializedOrder = JsonSerializer.Serialize(order);
            HttpContext.Session.SetString("pendingOrder", serializedOrder);
            HttpContext.Session.SetString("numbermakeOrder", order.OrderCode);
            HttpContext.Session.SetString("makeOrder", "true");

            _cartService.Clear();

            var langId = Request.Cookies["LangId"] ?? "vi";
            var config = _rootService.GetConfig(langId);

            if (string.Equals(PaymentMethod, "vietqr", StringComparison.OrdinalIgnoreCase))
            {
                var bank = config._BankKing;
                if (bank == null || string.IsNullOrEmpty(bank.Title) || string.IsNullOrEmpty(bank.Number))
                {
                    TempData["CheckoutError"] = "Chưa cấu hình tài khoản VietQR.";
                    return Redirect(fallback);
                }

                var qrUrl = _vietQrService.GenerateQRCode(bank, total, order.OrderCode).Result ?? string.Empty;
                HttpContext.Session.SetString("qrCodeUrl", qrUrl);
                return Redirect("/vietqr");
            }

            if (string.Equals(PaymentMethod, "vnpay", StringComparison.OrdinalIgnoreCase))
            {
                var vnPay = config.VNPay;
                if (vnPay == null || string.IsNullOrEmpty(vnPay.VNP_TmnCode) ||
                    string.IsNullOrEmpty(vnPay.VNP_HashSecret) || string.IsNullOrEmpty(vnPay.VNP_Url))
                {
                    TempData["CheckoutError"] = "Chưa cấu hình VNPay.";
                    return Redirect(fallback);
                }

                var service = new VNPayService(vnPay.VNP_TmnCode, vnPay.VNP_HashSecret, vnPay.VNP_Url);
                var returnUrl = vnPay.LinkVPN ?? $"{Request.Scheme}://{Request.Host}/VNPay/Return";
                var paymentUrl = service.CreatePaymentUrl(order.OrderCode, total, returnUrl, HttpContext.Connection.RemoteIpAddress?.ToString());
                return Redirect(paymentUrl);
            }

            var successPage = config.Link?.PageInfoOrder ?? "/order-success";
            TempData["CheckoutSuccess"] = "Đơn hàng đã được tạo thành công!";
            return Redirect($"{successPage}?code={order.OrderCode}");
        }
    }
}

