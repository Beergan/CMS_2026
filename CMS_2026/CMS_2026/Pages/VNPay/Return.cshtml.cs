using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CMS_2026.Data;
using CMS_2026.Data.Entities;
using CMS_2026.Services;
using CMS_2026.Utils;

namespace CMS_2026.Pages.VNPay
{
    public class ReturnModel : PageModel
    {
        private readonly IDataService _dataService;
        private readonly RootService _rootService;

        public ReturnModel(IDataService dataService, RootService rootService)
        {
            _dataService = dataService;
            _rootService = rootService;
        }

        public string? OrderCode { get; set; }
        public string? Message { get; set; }
        public bool IsSuccess { get; set; }

        public IActionResult OnGet()
        {
            var langId = Request.Cookies["LangId"] ?? "vi";
            var config = _rootService.GetConfig(langId);
            var vnPay = config.VNPay;

            if (vnPay == null || string.IsNullOrEmpty(vnPay.VNP_TmnCode) ||
                string.IsNullOrEmpty(vnPay.VNP_HashSecret) || string.IsNullOrEmpty(vnPay.VNP_Url))
            {
                Message = "Chưa cấu hình VNPay.";
                IsSuccess = false;
                return Page();
            }

            var service = new VNPayService(vnPay.VNP_TmnCode, vnPay.VNP_HashSecret, vnPay.VNP_Url);

            // Process VNPay response
            foreach (var key in Request.Query.Keys)
            {
                service.AddResponseData(key, Request.Query[key].ToString());
            }

            var secureHash = Request.Query["vnp_SecureHash"].ToString();
            if (string.IsNullOrEmpty(secureHash))
            {
                Message = "Thiếu thông tin xác thực từ VNPay.";
                IsSuccess = false;
                return Page();
            }

            if (!service.ValidateSignature(secureHash))
            {
                Message = "Chữ ký không hợp lệ.";
                IsSuccess = false;
                return Page();
            }

            var responseCode = service.GetResponseData("vnp_ResponseCode");
            OrderCode = service.GetResponseData("vnp_TxnRef");

            if (responseCode == "00" && !string.IsNullOrEmpty(OrderCode))
            {
                // Payment successful
                var order = _dataService.GetOne<PP_Order>(o => o.OrderCode == OrderCode);
                if (order != null)
                {
                    order.OrderStatus = "PAID";
                    order.PayMethod = "VNPay";
                    _dataService.Update(order);
                    _dataService.SaveChanges();

                    // Store order info in session for success page
                    var serializedOrder = System.Text.Json.JsonSerializer.Serialize(order);
                    HttpContext.Session.SetString("pendingOrder", serializedOrder);
                    HttpContext.Session.SetString("numbermakeOrder", order.OrderCode);
                    HttpContext.Session.SetString("makeOrder", "true");

                    IsSuccess = true;
                    Message = "Thanh toán thành công!";
                    
                    var successPage = config.Link?.PageInfoOrder ?? "/order-success";
                    return Redirect($"{successPage}?code={OrderCode}");
                }
                else
                {
                    Message = "Không tìm thấy đơn hàng.";
                    IsSuccess = false;
                }
            }
            else
            {
                Message = $"Thanh toán thất bại. Mã lỗi: {responseCode}";
                IsSuccess = false;
            }

            return Page();
        }
    }
}

