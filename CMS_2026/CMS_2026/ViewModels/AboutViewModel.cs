 using CMS_2026.Attributes;
using CMS_2026.Common;
using static CMS_2026.Common.InputControlType;

namespace CMS_2026.ViewModels
{
    [Component(
        Type = "Page_Template",
        ComptName = "en:Page about template|vi:Mẫu trang giới thiệu")]
    public class AboutViewModel
    {
        public string? Title { get; set; }
        public string? MetaDescription { get; set; }
        public string? MetaKeywords { get; set; }

        public class About
        {
            [Field(Title = "Tiêu đề", Required = false, Control = InputControlType.TextBox)]
            public string? Title { get; set; }

            [Field(Title = "Nội dung", Required = false, Control = InputControlType.TextArea)]
            public string? Content { get; set; }

            [Field(Title = "Hình ảnh", Required = false, Control = InputControlType.Image)]
            public string? Image { get; set; }
        }

        [Field(Title = "Giới thiệu")]
        public About? _About { get; set; }

        public class About2
        {
            [Field(Title = "Tiêu đề", Required = false, Control = TextBox)]
            public string? Titleabout2 { get; set; }

            [Field(Title = "Mô tả", Required = false, Control = TextArea)]
            public string? Content { get; set; }

            public class MilestoneSection
            {
                [Field(Title = "Hình ảnh", Required = false, Control = InputControlType.Image)]
                public string? Iamge { get; set; }

                [Field(Title = "Tiêu đề", Required = false, Control = InputControlType.TextBox)]
                public string? Title { get; set; }

                [Field(Title = "Mô tả", Required = false, Control = InputControlType.TextArea)]
                public string? Content { get; set; }
            }

            [Field(Title = "Cột mốc")]
            public MilestoneSection[]? _MilestoneSection { get; set; }

            public class ListImages
            {
                [Field(Title = "Hình ảnh", Required = false, Control = InputControlType.Image)]
                public string? Iamge { get; set; }

                [Field(Title = "Tiêu đề", Required = false, Control = InputControlType.TextBox)]
                public string? Title { get; set; }
            }

            [Field(Title = "Hình ảnh")]
            public ListImages[]? _ListImages { get; set; }
        }

        [Field(Title = "Giới thiệu 2")]
        public About2? _About2 { get; set; }

            public class Team
            {
                [Field(Title = "Tiêu đề", Required = false, Control = InputControlType.TextBox)]
                public string? Titleteam { get; set; }

                [Field(Title = "Mô tả", Required = false, Control = InputControlType.TextArea)]
                public string? Content2 { get; set; }

                public class List
                {
                    [Field(Title = "Chức vụ", Required = false, Control = InputControlType.TextBox)]
                    public string? Position { get; set; }

                    [Field(Title = "Tên", Required = false, Control = InputControlType.TextBox)]
                    public string? Title { get; set; }

                    [Field(Title = "Mô tả", Required = false, Control = InputControlType.TextArea)]
                    public string? Content { get; set; }

                    [Field(Title = "Hình ảnh", Required = false, Control = InputControlType.Image)]
                    public string? Image { get; set; }

                    [Field(Title = "link Twitter", Required = false, Control = InputControlType.TextBox)]
                    public string? Twitter { get; set; }

                    [Field(Title = "Link Facebook", Required = false, Control = InputControlType.TextBox)]
                    public string? Facebook { get; set; }

                    [Field(Title = "Link Instagram", Required = false, Control = InputControlType.TextBox)]
                    public string? Instagram { get; set; }

                    [Field(Title = "Link Youtube", Required = false, Control = InputControlType.TextBox)]
                    public string? Youtube { get; set; }
                }

            [Field(Title = "Danh sách")]
            public List[]? _List { get; set; }
        }

        [Field(Title = "Nhân sự")]
        public Team? _Team { get; set; }

        public static AboutViewModel Default => new AboutViewModel
        {
            Title = "Giới thiệu"
        };
    }
}

