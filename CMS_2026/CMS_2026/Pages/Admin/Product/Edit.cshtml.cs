using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CMS_2026.Data.Entities;
using CMS_2026.Services;
using CMS_2026.Utils;

namespace CMS_2026.Pages.Admin.Product
{
    public class EditModel : BaseAdminPageModel
    {
        public PP_Product? Product { get; set; }
        public Dictionary<string, string> GroupSelector { get; set; } = new();

        public EditModel(IDataService dataService, RootService rootService, 
            Microsoft.Extensions.Caching.Memory.IMemoryCache cache, PermissionService permissionService)
            : base(dataService, rootService, cache, permissionService)
        {
        }

        public IActionResult OnGet(int? id)
        {
            if (!id.HasValue)
            {
                return Redirect("/admin/product");
            }

            Product = Db.GetOne<PP_Product>(id.Value);
            if (Product == null)
            {
                return Redirect("/admin/product");
            }

            GroupSelector = GetGroupSelector(LangIdCompose, "product");

            return Page();
        }

        public IActionResult OnPost([FromForm] int Id, [FromForm] int CategoryId, 
            [FromForm] string Title, [FromForm] string NodePath, [FromForm] string? Des, 
            [FromForm] string? Content, [FromForm] string? ImageUrl, [FromForm] string? ProductCode,
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

                var product = Db.GetOne<PP_Product>(Id);
                if (product == null)
                {
                    return new JsonResult(new { success = false, message = "Không tìm thấy sản phẩm!" });
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

                if (tempAlias != product.NodePath && 
                    Db.GetList<PP_Product>(t => t.NodePath == tempAlias && t.LangId == LangIdCompose && t.Id != Id).Any())
                {
                    return new JsonResult(new { success = false, message = $"Đường dẫn [{tempAlias}] đã tồn tại!" });
                }

                product.CategoryId = CategoryId;
                product.Title = Title;
                product.NodePath = tempAlias;
                product.Des = Des;
                product.Content = Content;
                product.ImageUrl = ImageUrl;
                product.ProductCode = ProductCode;
                product.Price = Price;
                product.PromotionPrice = PromotionPrice;
                product.PromotionEnabled = PromotionEnabled;
                product.BestSeller = BestSeller;
                product.StockQty = StockQty;
                product.Weight = Weight;
                product.MetaDescription = MetaDescription;
                product.MetaKeywords = MetaKeywords;

                Db.Update(product);

                // Update category details
                var catDetails = Db.GetOne<PP_Category_details>(t => t.Idproduct == Id && t.NodeType == "product");
                if (catDetails != null)
                {
                    catDetails.Idcat = CategoryId;
                    Db.Update(catDetails);
                }
                else
                {
                    catDetails = new PP_Category_details
                    {
                        Idcat = CategoryId,
                        LangId = LangIdCompose,
                        PageId = category.PageId,
                        NodeType = "product",
                        PageIdItem = category.PageIdItem,
                        Idproduct = product.Id
                    };
                    Db.Insert(catDetails);
                }

                Root.ClearCache();

                return new JsonResult(new { success = true, message = "Cập nhật thành công!", redirect = "/admin/product" });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }
    }
}

