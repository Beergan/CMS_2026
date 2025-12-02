using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CMS_2026.Data.Entities;
using CMS_2026.Services;
using CMS_2026.Pages.Admin;
using System.Text.Json;

namespace CMS_2026.Pages.Admin.Product
{
    public class ImageModel : BaseAdminPageModel
    {
        public PP_Product? Product { get; set; }
        public List<string> ImageUrls { get; set; } = new();

        public ImageModel(IDataService dataService, RootService rootService, 
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

            // Parse images from JSON
            if (!string.IsNullOrEmpty(Product.ImagesJson))
            {
                try
                {
                    ImageUrls = JsonSerializer.Deserialize<List<string>>(Product.ImagesJson) ?? new();
                }
                catch
                {
                    ImageUrls = new();
                }
            }

            return Page();
        }

        public IActionResult OnPostAppend([FromForm] int Id, [FromForm] string ImageUrl)
        {
            try
            {
                var product = Db.GetOne<PP_Product>(Id);
                if (product == null)
                {
                    return new JsonResult(new { success = false, message = "Sản phẩm không tồn tại!" });
                }

                if (string.IsNullOrEmpty(ImageUrl))
                {
                    return new JsonResult(new { success = false, message = "URL hình ảnh không được để trống!" });
                }

                var imageUrls = new List<string>();
                if (!string.IsNullOrEmpty(product.ImagesJson))
                {
                    try
                    {
                        imageUrls = JsonSerializer.Deserialize<List<string>>(product.ImagesJson) ?? new();
                    }
                    catch { }
                }

                if (!imageUrls.Contains(ImageUrl))
                {
                    imageUrls.Add(ImageUrl);
                    product.ImagesJson = JsonSerializer.Serialize(imageUrls);
                    Db.Update(product);
                    Db.SaveChanges();
                    Root.ClearCache();
                }

                return new JsonResult(new { success = true, message = "Thêm hình thành công!" });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }

        public IActionResult OnPostRemove([FromForm] int Id, [FromForm] string ImageUrl)
        {
            try
            {
                var product = Db.GetOne<PP_Product>(Id);
                if (product == null)
                {
                    return new JsonResult(new { success = false, message = "Sản phẩm không tồn tại!" });
                }

                var imageUrls = new List<string>();
                if (!string.IsNullOrEmpty(product.ImagesJson))
                {
                    try
                    {
                        imageUrls = JsonSerializer.Deserialize<List<string>>(product.ImagesJson) ?? new();
                    }
                    catch { }
                }

                if (imageUrls.Contains(ImageUrl))
                {
                    imageUrls.Remove(ImageUrl);
                    product.ImagesJson = imageUrls.Any() ? JsonSerializer.Serialize(imageUrls) : null;
                    
                    // Set first image as main image if exists
                    if (imageUrls.Any())
                    {
                        product.ImageUrl = imageUrls[0];
                    }

                    Db.Update(product);
                    Db.SaveChanges();
                    Root.ClearCache();
                }

                return new JsonResult(new { success = true, message = "Xóa hình thành công!" });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }
    }
}

