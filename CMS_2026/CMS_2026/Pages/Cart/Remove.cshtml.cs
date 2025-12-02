using CMS_2026.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CMS_2026.Pages.Cart
{
    public class RemoveModel : PageModel
    {
        private readonly IDataService _dataService;
        private readonly ShoppingCartService _cartService;

        public RemoveModel(IDataService dataService, ShoppingCartService cartService)
        {
            _dataService = dataService;
            _cartService = cartService;
        }

        public IActionResult OnPost([FromForm] int ProductId, [FromForm] string? Variation)
        {
            var referer = Request.Headers["Referer"].ToString();
            var fallback = string.IsNullOrEmpty(referer) ? "/cart" : referer;

            var product = _dataService.GetOne<Data.Entities.PP_Product>(ProductId);
            if (product == null)
            {
                TempData["CartError"] = "Sản phẩm không tồn tại.";
                return Redirect(fallback);
            }

            _cartService.UpdateOne(product, Variation, 0);
            TempData["CartMessage"] = "Đã xóa sản phẩm khỏi giỏ hàng.";
            return Redirect(fallback);
        }
    }
}

