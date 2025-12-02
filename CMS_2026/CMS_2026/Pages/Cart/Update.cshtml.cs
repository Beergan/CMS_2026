using CMS_2026.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CMS_2026.Pages.Cart
{
    public class UpdateModel : PageModel
    {
        private readonly IDataService _dataService;
        private readonly ShoppingCartService _cartService;

        public UpdateModel(IDataService dataService, ShoppingCartService cartService)
        {
            _dataService = dataService;
            _cartService = cartService;
        }

        public IActionResult OnPost([FromForm] int ProductId, [FromForm] int Quantity, [FromForm] string? Variation)
        {
            var referer = Request.Headers["Referer"].ToString();
            var fallback = string.IsNullOrEmpty(referer) ? "/cart" : referer;

            var product = _dataService.GetOne<Data.Entities.PP_Product>(ProductId);
            if (product == null)
            {
                TempData["CartError"] = "Sản phẩm không tồn tại.";
                return Redirect(fallback);
            }

            if (Quantity < 0) Quantity = 0;

            _cartService.UpdateOne(product, Variation, Quantity);
            TempData["CartMessage"] = "Cập nhật giỏ hàng thành công.";
            return Redirect(fallback);
        }
    }
}

