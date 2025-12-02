using CMS_2026.Attributes;
using static CMS_2026.Common.InputControlType;

namespace CMS_2026.ViewModels
{
    [Component(
        Type = "Page_Template",
        ComptName = "en:Page contact template|vi:Mẫu trang liên hệ")]
    public class ContactViewModel
    {
        public string? Title { get; set; }
        public string? MetaDescription { get; set; }
        public string? MetaKeywords { get; set; }

        [Field(Title = "Tiêu đề liên hệ", Required = false, Control = TextBox)]
        public string? _TitleMain { get; set; }

        [Field(Title = "Mô tả liên hệ", Required = false, Control = RichTextBox)]
        public string? _ContentContact { get; set; }

        [Field(Title = "Địa chỉ", Required = false, Control = RichTextBox)]
        public string? _Address { get; set; }

        [Field(Title = "Thông tin liên hệ", Required = false, Control = RichTextBox)]
        public string? _InforContact { get; set; }

        [Field(Title = "Giờ làm việc", Required = false, Control = RichTextBox)]
        public string? _WorkingTime { get; set; }

        public static ContactViewModel Default => new ContactViewModel
        {
            Title = "Liên hệ"
        };
    }
}

