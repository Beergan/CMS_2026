using CMS_2026.Attributes;
using CMS_2026.Common;
using CMS_2026.Data.Entities;

namespace CMS_2026.ViewModels
{
    [Component(
        Type = "Page_Template",
        ComptName = "en:Homepage template|vi:Mẫu trang chủ",
        PageType = "single"
    )]
    public class HomeViewModel
    {
        public string? Title { get; set; }
        public string? MetaDescription { get; set; }
        public string? MetaKeywords { get; set; }

        public class Banner
        {
            [Field(Title = "Hình ảnh", Control = InputControlType.Image)]
            public string? Iamge { get; set; }
            
            // Alias for backward compatibility
            public string? Image { get => Iamge; set => Iamge = value; }
            
            [Field(Title = "Hình ảnh Mobile", Control = InputControlType.Image)]
            public string? IamgeMobile { get; set; }
            
            // Alias for backward compatibility
            public string? ImageMobile { get => IamgeMobile; set => IamgeMobile = value; }
            
            [Field(Title = "Tiêu đề phụ", Control = InputControlType.TextBox)]
            public string? TitleSub { get; set; }
            
            [Field(Title = "Tiêu đề", Control = InputControlType.TextBox)]
            public string? Title { get; set; }
            
            [Field(Title = "Mô tả", Control = InputControlType.TextArea)]
            public string? Content { get; set; }
            
            [Field(Title = "Tiêu đề nút", Control = InputControlType.TextBox)]
            public string? titleButton { get; set; }
            
            // Alias for backward compatibility
            public string? TitleButton { get => titleButton; set => titleButton = value; }
            
            [Field(Title = "Đường dẫn", Control = InputControlType.Link)]
            public string? Link { get; set; }
        }

        [Field(Title = "Banner")]
        public Banner[]? _Banner { get; set; }
        
        // Alias for backward compatibility
        public Banner[]? Banners { get => _Banner; set => _Banner = value; }

        public class Product
        {
            [Field(Title = "Tiêu đề", Control = InputControlType.TextBox)]
            public string? title { get; set; }
            
            // Alias for backward compatibility
            public string? Title { get => title; set => title = value; }
            
            [Field(Title = "Mô tả", Control = InputControlType.TextArea)]
            public string? Content { get; set; }
            
            [Field(Title = "Đường dẫn", Control = InputControlType.Link)]
            public string? Link { get; set; }
            
            [Field(Title = "Số lượng sản phẩm", Control = InputControlType.Number)]
            public int Count { get; set; }
        }

        [Field(Title = "Sản phẩm")]
        public Product? _Product { get; set; }
        
        // Alias for backward compatibility
        public Product? ProductSection { get => _Product; set => _Product = value; }

        public class Advertisement
        {
            [Field(Title = "Hình ảnh(690x420)", Control = InputControlType.Image)]
            public string? Iamge { get; set; }
            
            // Alias for backward compatibility
            public string? Image { get => Iamge; set => Iamge = value; }
            
            [Field(Title = "Tiêu đề phụ", Control = InputControlType.TextBox)]
            public string? Titlesub { get; set; }
            
            // Alias for backward compatibility
            public string? TitleSub { get => Titlesub; set => Titlesub = value; }
            
            [Field(Title = "Tiêu đề", Control = InputControlType.TextBox)]
            public string? title { get; set; }
            
            // Alias for backward compatibility
            public string? Title { get => title; set => title = value; }
            
            [Field(Title = "Đường dẫn", Control = InputControlType.Link)]
            public string? Link { get; set; }
            
            [Field(Title = "Hình ảnh 2(690x420)", Control = InputControlType.Image)]
            public string? Iamge2 { get; set; }
            
            // Alias for backward compatibility
            public string? Image2 { get => Iamge2; set => Iamge2 = value; }
            
            [Field(Title = "Tiêu đề 2", Control = InputControlType.TextBox)]
            public string? Title2 { get; set; }
            
            [Field(Title = "Mô tả 2", Control = InputControlType.TextBox)]
            public string? Content { get; set; }
            
            [Field(Title = "Đường dẫn 2", Control = InputControlType.Link)]
            public string? Link2 { get; set; }
        }

        [Field(Title = "Quảng bá")]
        public Advertisement? _Advertisement { get; set; }
        
        // Alias for backward compatibility
        public Advertisement? AdvertisementSection { get => _Advertisement; set => _Advertisement = value; }

        public class Service
        {
            [Field(Title = "Phí ship", Control = InputControlType.TextBox)]
            public string? Title { get; set; }
            
            [Field(Title = "Mô tả phí ship", Control = InputControlType.TextBox)]
            public string? Content1 { get; set; }
            
            [Field(Title = "Hoàn trả", Control = InputControlType.TextBox)]
            public string? Title2 { get; set; }
            
            [Field(Title = "Mô tả hoàn trả", Control = InputControlType.TextBox)]
            public string? Content2 { get; set; }
            
            [Field(Title = "Hỗ trợ", Control = InputControlType.TextBox)]
            public string? Title3 { get; set; }
            
            [Field(Title = "Mô tả hỗ trợ", Control = InputControlType.TextBox)]
            public string? Content3 { get; set; }
            
            [Field(Title = "Thanh toán", Control = InputControlType.TextBox)]
            public string? Title4 { get; set; }
            
            [Field(Title = "Mô tả thanh toán", Control = InputControlType.TextBox)]
            public string? Content4 { get; set; }
        }

        [Field(Title = "Dịch vụ")]
        public Service? _Service { get; set; }
        
        // Alias for backward compatibility
        public Service? ServiceSection { get => _Service; set => _Service = value; }

        public class ProductBestseller
        {
            [Field(Title = "Tiêu đề", Control = InputControlType.TextBox)]
            public string? title { get; set; }
            
            // Alias for backward compatibility
            public string? Title { get => title; set => title = value; }
            
            [Field(Title = "Mô tả", Control = InputControlType.TextArea)]
            public string? Content { get; set; }
            
            [Field(Title = "Đường dẫn", Control = InputControlType.Link)]
            public string? Link { get; set; }
            
            [Field(Title = "SL Sản phẩm", Control = InputControlType.Number)]
            public int Count { get; set; }
            
            [Field(Title = "Hình ảnh", Control = InputControlType.Image)]
            public string? Iamge { get; set; }
            
            // Alias for backward compatibility
            public string? Image { get => Iamge; set => Iamge = value; }
            
            [Field(Title = "Tiêu đề hình ảnh", Control = InputControlType.TextBox)]
            public string? Titleiamge { get; set; }
            
            // Alias for backward compatibility
            public string? TitleImage { get => Titleiamge; set => Titleiamge = value; }
            
            [Field(Title = "Mô tả hình ảnh", Control = InputControlType.TextArea)]
            public string? ContentImage { get; set; }
        }

        [Field(Title = "Sản phẩm bán chạy")]
        public ProductBestseller? _ProductBestseller { get; set; }
        
        // Alias for backward compatibility
        public ProductBestseller? ProductBestsellerSection { get => _ProductBestseller; set => _ProductBestseller = value; }

        public class Discover
        {
            [Field(Title = "Tiêu đề", Control = InputControlType.TextBox)]
            public string? title { get; set; }
            
            // Alias for backward compatibility
            public string? Title { get => title; set => title = value; }
            
            [Field(Title = "Mô tả", Control = InputControlType.TextArea)]
            public string? Content { get; set; }

            public class listImage
            {
                [Field(Title = "Tiêu đề", Control = InputControlType.TextBox)]
                public string? title { get; set; }
                
                // Alias for backward compatibility
                public string? Title { get => title; set => title = value; }
                
                [Field(Title = "Hình ảnh", Control = InputControlType.Image)]
                public string? Image { get; set; }
                
                [Field(Title = "Đường dẫn", Control = InputControlType.Link)]
                public string? Link { get; set; }
            }

            [Field(Title = "Khuyến mãi")]
            public listImage[]? _listImage { get; set; }
            
            // Alias for backward compatibility
            public listImage[]? ListImages { get => _listImage; set => _listImage = value; }
        }

        [Field(Title = "Khuyến mãi")]
        public Discover? _Discover { get; set; }
        
        // Alias for backward compatibility
        public Discover? DiscoverSection { get => _Discover; set => _Discover = value; }

        public class AlbumIamge
        {
            [Field(Title = "Tiêu đề", Control = InputControlType.TextBox)]
            public string? title { get; set; }
            
            // Alias for backward compatibility
            public string? Title { get => title; set => title = value; }
            
            [Field(Title = "Hình ảnh", Control = InputControlType.Image)]
            public string? Image { get; set; }
        }
        
        // Alias for backward compatibility
        public class AlbumImage : AlbumIamge { }

        [Field(Title = "Album Ảnh")]
        public AlbumImage[]? _AlbumIamge { get; set; }
        
        // Alias for backward compatibility
        public AlbumImage[]? AlbumImages { get => _AlbumIamge; set => _AlbumIamge = value; }

        // Product data models (similar to old structure)
        public PP_Category[]? _modelCat { get; set; }
        public PP_Product[]? _modelProduct { get; set; }
        public PP_Node[]? _modelNode { get; set; }
        public VM_Product[]? _modelVMProduct { get; set; }
        public VM_Blog[]? _modelVBlog { get; set; }

        public string? CategoryPath { get; set; }
    }

    // Placeholder for VM_Blog (if needed)
    public class VM_Blog
    {
        public string? Title { get; set; }
        public string? NodePath { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime CreatedTime { get; set; }
        public int? CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public string? CategoryPath { get; set; }
        public string? Summary { get; set; }
    }
}

