using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using CMS_2026.Models;

namespace CMS_2026.Services
{
    public class VietQRService
    {
        private readonly HttpClient _httpClient;

        public VietQRService()
        {
            _httpClient = new HttpClient();
        }

        public async Task<string?> GenerateQRCode(BankKing bankInfo, decimal amount, string orderCode, string? description = null)
        {
            try
            {
                // VietQR API endpoint (example - adjust based on actual API)
                var apiUrl = "https://img.vietqr.io/image/";
                
                // Build QR code URL for VietQR
                // Format: https://img.vietqr.io/image/{bankCode}-{accountNumber}-{amount}.png
                var qrUrl = $"{apiUrl}{bankInfo.Title}-{bankInfo.Number}-{amount}.png";
                
                // Alternative: Use VietQR API to generate QR code
                // This is a simplified version - actual implementation may require API key
                var requestData = new
                {
                    bankCode = bankInfo.Title,
                    accountNumber = bankInfo.Number,
                    accountName = bankInfo.Name,
                    amount = amount,
                    description = description ?? $"Thanh toan don hang {orderCode}",
                    template = "compact"
                };

                // For now, return a simple QR code URL
                // In production, you would call VietQR API
                return qrUrl;
            }
            catch
            {
                return null;
            }
        }

        public string GenerateQRCodeUrl(BankKing bankInfo, decimal amount, string orderCode)
        {
            // Simple QR code generation using VietQR format
            var qrData = $"00020101021238570010A00000072701270006{bankInfo.Title}0108{bankInfo.Number}0208{amount:00000000}0304{orderCode}53037045404{amount}5802VN6207{orderCode}6304";
            
            // Return URL to QR code generator service
            return $"https://api.vietqr.io/v2/generate?accountNo={bankInfo.Number}&accountName={bankInfo.Name}&acqId={bankInfo.Title}&amount={amount}&addInfo={orderCode}";
        }
    }
}

