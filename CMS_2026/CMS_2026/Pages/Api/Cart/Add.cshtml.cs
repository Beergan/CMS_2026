using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CMS_2026.Services;
using CMS_2026.Data;

namespace CMS_2026.Pages.Api.Cart
{
    [IgnoreAntiforgeryToken]
    public class AddModel : PageModel
    {
        private readonly ShoppingCartService _cartService;
        private readonly IDataService _dataService;

        public AddModel(ShoppingCartService cartService, IDataService dataService)
        {
            _cartService = cartService;
            _dataService = dataService;
        }

        public IActionResult OnPost([FromForm] int productId, [FromForm] string? variation = null, [FromForm] int quantity = 1)
        {
            try
            {
                var product = _dataService.GetOne<Data.Entities.PP_Product>(productId);
                if (product == null)
                {
                    return new JsonResult(new { success = false, message = "Sản phẩm không tồn tại." });
                }

                _cartService.AddItem(product, variation, quantity);
                var cartCount = _cartService.GetItemCount();
                var cartTotal = _cartService.GetTotal();

                return new JsonResult(new
                {
                    success = true,
                    message = "Đã thêm vào giỏ hàng!",
                    cartCount = cartCount,
                    cartTotal = cartTotal
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }
    }
}

