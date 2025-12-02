using CMS_2026.Attributes;
using CMS_2026.Common;
using static CMS_2026.Common.InputControlType;

namespace CMS_2026.ViewModels
{
    [Component(
        Type = "Page_Element",
        ComptName = "en:Page footer component|vi:Thành phần chân trang"
    )]
    public class FooterViewModel
    {
        [Field(
            Title = "Tiêu đề công ty",
            Required = false,
            Control = TextBox)]
        public string? Titlemain { get; set; }

        [Field(
            Title = "Môt tả công ty",
            Required = false,
            Control = RichTextBox)]
        public string? Des { get; set; }

        public class Link
        {
            [Field(
                Title = "Tiêu đề",
                Required = false,
                Control = TextBox)]
            public string? Title { get; set; }

            [Field(
                Title = "Liên kết",
                Required = false,
                Control = InputControlType.Link)]
            public string? link { get; set; }
        }

        [Field(Title = "Link Menu")]
        public Link[]? link { get; set; }

        public class Link1
        {
            [Field(
                Title = "Tiêu đề",
                Required = false,
                Control = TextBox)]
            public string? Title { get; set; }

            [Field(
                Title = "Liên kết",
                Required = false,
                Control = InputControlType.Link)]
            public string? link { get; set; }
        }

        [Field(Title = "Thông tin cần biết")]
        public Link1[]? link1 { get; set; }

        public class Newsletter
        {
            [Field(
                Title = "Tiêu đề",
                Required = false,
                Control = TextBox)]
            public string? Title { get; set; }

            [Field(
                Title = "Mô tả",
                Required = false,
                Control = TextBox)]
            public string? Content { get; set; }
        }

        [Field(Title = "Email")]
        public Newsletter? newsletter { get; set; }

        public static FooterViewModel Default => new FooterViewModel();
    }
}

