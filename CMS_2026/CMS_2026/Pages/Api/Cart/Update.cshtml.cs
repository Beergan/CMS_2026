using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CMS_2026.Services;
using CMS_2026.Data.Entities;

namespace CMS_2026.Pages.Api.Cart
{
    [IgnoreAntiforgeryToken]
    public class UpdateModel : PageModel
    {
        private readonly ShoppingCartService _cartService;
        private readonly IDataService _dataService;

        public UpdateModel(ShoppingCartService cartService, IDataService dataService)
        {
            _cartService = cartService;
            _dataService = dataService;
        }

        public IActionResult OnPost([FromForm] int productId, [FromForm] string? variation = null, [FromForm] int quantity = 1)
        {
            try
            {
                var product = _dataService.GetOne<PP_Product>(productId);
                if (product == null)
                {
                    return new JsonResult(new { success = false, message = "Sản phẩm không tồn tại!" });
                }
                _cartService.UpdateQuantity(product, variation, quantity);
                var cartCount = _cartService.GetItemCount();
                var cartTotal = _cartService.GetTotal();
                var subTotal = _cartService.GetSubTotal();
                var shipFee = _cartService.ShipFee;

                return new JsonResult(new
                {
                    success = true,
                    cartCount = cartCount,
                    cartTotal = cartTotal,
                    subTotal = subTotal,
                    shipFee = shipFee
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }
    }
}

