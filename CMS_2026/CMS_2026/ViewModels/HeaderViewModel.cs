using CMS_2026.Attributes;
using CMS_2026.Common;

namespace CMS_2026.ViewModels
{
    [Component(
        Type = "Layout_Component",
        ComptName = "en:Header template|vi:Mẫu header",
        PageType = "layout"
    )]
    public class HeaderViewModel
    {
        [Field(Title = "Logo", Control = InputControlType.Image)]
        public string? Logo { get; set; }

        [Field(Title = "Logo Mobile", Control = InputControlType.Image)]
        public string? LogoMobile { get; set; }

        [Field(Title = "Số điện thoại", Control = InputControlType.TextBox)]
        public string? Phone { get; set; }

        [Field(Title = "Email", Control = InputControlType.TextBox)]
        public string? Email { get; set; }

        [Field(Title = "Địa chỉ", Control = InputControlType.TextArea)]
        public string? Address { get; set; }

        public class MenuItem
        {
            [Field(Title = "Tiêu đề menu", Control = InputControlType.TextBox)]
            public string? Title { get; set; }

            [Field(Title = "Liên kết", Control = InputControlType.Link)]
            public string? Link { get; set; }

            [Field(Title = "Icon", Control = InputControlType.Icon)]
            public string? Icon { get; set; }

            [Field(Title = "Mở tab mới", Control = InputControlType.CheckBox)]
            public bool OpenNewTab { get; set; }
        }

        [Field(Title = "Menu chính", ChildTitle = "Menu item")]
        public MenuItem[]? MainMenu { get; set; }

        public class SocialLink
        {
            [Field(Title = "Tên mạng xã hội", Control = InputControlType.TextBox)]
            public string? Name { get; set; }

            [Field(Title = "Icon", Control = InputControlType.Icon)]
            public string? Icon { get; set; }

            [Field(Title = "Liên kết", Control = InputControlType.Link)]
            public string? Link { get; set; }
        }

        [Field(Title = "Mạng xã hội", ChildTitle = "Social link")]
        public SocialLink[]? SocialLinks { get; set; }

        [Field(Title = "Hotline", Control = InputControlType.TextBox)]
        public string? Hotline { get; set; }

        [Field(Title = "Text hotline", Control = InputControlType.TextBox)]
        public string? HotlineText { get; set; }

        public static HeaderViewModel Default => new HeaderViewModel
        {
            Logo = "/assets/images/logo.png",
            LogoMobile = "/assets/images/logo-mobile.png",
            Phone = "0123456789",
            Email = "info@example.com",
            Address = "123 Đường ABC, Quận XYZ, TP.HCM",
            Hotline = "19001234",
            HotlineText = "Hotline: 1900 1234",
            MainMenu = new MenuItem[]
            {
                new MenuItem { Title = "Trang chủ", Link = "/" },
                new MenuItem { Title = "Giới thiệu", Link = "/about" },
                new MenuItem { Title = "Sản phẩm", Link = "/products" },
                new MenuItem { Title = "Tin tức", Link = "/blog" },
                new MenuItem { Title = "Liên hệ", Link = "/contact" }
            },
            SocialLinks = new SocialLink[]
            {
                new SocialLink { Name = "Facebook", Icon = "bi bi-facebook", Link = "https://facebook.com" },
                new SocialLink { Name = "Instagram", Icon = "bi bi-instagram", Link = "https://instagram.com" }
            }
        };
    }
}

