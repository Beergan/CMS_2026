using CMS_2026.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CMS_2026.Pages.Cart
{
    public class AddModel : PageModel
    {
        private readonly IDataService _dataService;
        private readonly ShoppingCartService _cartService;

        public AddModel(IDataService dataService, ShoppingCartService cartService)
        {
            _dataService = dataService;
            _cartService = cartService;
        }

        public IActionResult OnPost([FromForm] int ProductId, [FromForm] int Quantity = 1, [FromForm] string? Variation = null)
        {
            var referer = Request.Headers["Referer"].ToString();
            var fallback = string.IsNullOrEmpty(referer) ? "/cart" : referer;

            var product = _dataService.GetOne<Data.Entities.PP_Product>(ProductId);
            if (product == null)
            {
                TempData["CartError"] = "Sản phẩm không tồn tại.";
                return Redirect(fallback);
            }

            if (Quantity < 1) Quantity = 1;

            _cartService.AddToCart(product, Variation, Quantity);
            TempData["CartMessage"] = "Đã thêm sản phẩm vào giỏ hàng.";
            return Redirect(fallback);
        }
    }
}

