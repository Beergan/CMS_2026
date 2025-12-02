using CMS_2026.Attributes;
using CMS_2026.Common;
using static CMS_2026.Common.InputControlType;

namespace CMS_2026.ViewModels
{
    [Component(
        Type = "Page_Template",
        ComptName = "en:Page about template|vi:Mẫu trang đăng ký")]
    public class RegisterViewModel
    {
        public string? Title { get; set; }
        public string? MetaDescription { get; set; }
        public string? MetaKeywords { get; set; }

        public class Contact
        {
            [Field(Title = "Tiêu đề", Required = false, Control = InputControlType.TextBox)]
            public string? Title { get; set; }

            [Field(Title = "Nội dung", Required = false, Control = InputControlType.RichTextBox)]
            public string? Content { get; set; }

            [Field(Title = "Baner", Required = false, Control = InputControlType.Image)]
            public string? Baner { get; set; }

            [Field(Title = "Hình ảnh", Required = false, Control = InputControlType.Image)]
            public string? Image { get; set; }

            [Field(Title = "Đường dẫn điều khoản", Required = false, Control = InputControlType.Link)]
            public string? link { get; set; }
        }

        [Field(Title = "Đăng ký")]
        public Contact? contact { get; set; }

        public static RegisterViewModel Default => new RegisterViewModel
        {
            Title = "Giới thiệu"
        };
    }
}

