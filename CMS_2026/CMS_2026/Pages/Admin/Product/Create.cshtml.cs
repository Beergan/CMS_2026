using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CMS_2026.Data.Entities;
using CMS_2026.Services;
using CMS_2026.Utils;

namespace CMS_2026.Pages.Admin.Product
{
    public class CreateModel : BaseAdminPageModel
    {
        public Dictionary<string, string> GroupSelector { get; set; } = new();

        public CreateModel(IDataService dataService, RootService rootService, 
            Microsoft.Extensions.Caching.Memory.IMemoryCache cache, PermissionService permissionService)
            : base(dataService, rootService, cache, permissionService)
        {
        }

        public void OnGet(int? categoryId = null)
        {
            GroupSelector = GetGroupSelector(LangIdCompose, "product");
        }

        public IActionResult OnPost([FromForm] int CategoryId, [FromForm] string Title, 
            [FromForm] string NodePath, [FromForm] string? Des, [FromForm] string? Content,
            [FromForm] string? ImageUrl, [FromForm] string? ProductCode, 
            [FromForm] decimal Price, [FromForm] decimal? PromotionPrice, 
            [FromForm] bool PromotionEnabled, [FromForm] bool BestSeller,
            [FromForm] int StockQty, [FromForm] int Weight,
            [FromForm] string? MetaDescription, [FromForm] string? MetaKeywords)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Title) || string.IsNullOrWhiteSpace(NodePath) || CategoryId == 0)
                {
                    return new JsonResult(new { success = false, message = "Vui lòng điền đầy đủ thông tin!" });
                }

                var category = Db.GetOne<PP_Category>(CategoryId);
                if (category == null)
                {
                    return new JsonResult(new { success = false, message = "Chuyên mục không tồn tại!" });
                }

                var page = Db.GetOne<PP_Page>(category.PageIdItem);
                if (page == null)
                {
                    return new JsonResult(new { success = false, message = "Trang chi tiết không tồn tại!" });
                }

                var slug = EncodeHelper.SanitizeString(NodePath);
                var tempAlias = string.Format(page.PathPattern, slug);

                if (Db.GetList<PP_Product>(t => t.NodePath == tempAlias && t.LangId == LangIdCompose).Any())
                {
                    return new JsonResult(new { success = false, message = $"Đường dẫn [{tempAlias}] đã tồn tại!" });
                }

                var product = new PP_Product
                {
                    LangId = LangIdCompose,
                    NodeType = "product",
                    CategoryId = CategoryId,
                    Title = Title,
                    NodePath = tempAlias,
                    Des = Des,
                    Content = Content,
                    ImageUrl = ImageUrl,
                    ProductCode = ProductCode,
                    Price = Price,
                    PromotionPrice = PromotionPrice,
                    PromotionEnabled = PromotionEnabled,
                    BestSeller = BestSeller,
                    StockQty = StockQty,
                    Weight = Weight,
                    MetaDescription = MetaDescription,
                    MetaKeywords = MetaKeywords,
                    NodeStatus = "CREATED",
                    Brand = "NO_BRAND",
                    PageId = category.PageId,
                    PageIdItem = category.PageIdItem
                };

                Db.Insert(product);

                // Create category details
                var catDetails = new PP_Category_details
                {
                    Idcat = CategoryId,
                    LangId = LangIdCompose,
                    PageId = category.PageId,
                    NodeType = "product",
                    PageIdItem = category.PageIdItem,
                    Idproduct = product.Id
                };
                Db.Insert(catDetails);

                Root.ClearCache();

                return new JsonResult(new { success = true, message = "Tạo sản phẩm thành công!", redirect = "/admin/product" });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }
    }
}

