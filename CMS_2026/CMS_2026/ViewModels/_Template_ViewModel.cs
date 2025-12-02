using CMS_2026.Attributes;
using CMS_2026.Common;
using System.Collections.Generic;

namespace CMS_2026.ViewModels
{
    /// <summary>
    /// TEMPLATE: Copy this file and rename to create a new ViewModel
    /// 
    /// Steps to create a new ViewModel:
    /// 1. Copy this file and rename (e.g., ProductViewModel.cs)
    /// 2. Update the class name (e.g., ProductViewModel)
    /// 3. Add [Component] attribute with appropriate settings
    /// 4. Add properties with [Field] attributes
    /// 5. Optionally add nested classes for complex objects
    /// 6. Optionally add Default static property for default values
    /// 7. Run /admin/sync to sync to database
    /// </summary>
    
    // ComponentAttribute is OPTIONAL - if not provided, will auto-detect from class name
    // But it's recommended to specify for better control
    [Component(
        Type = "Page_Template",           // Page_Template, Layout_Component, etc.
        ComptName = "en:Template Name|vi:Tên Template",  // Multi-language name
        PageType = "single"                // single, list, item
    )]
    public class TemplateViewModel
    {
        // Simple fields
        [Field(Title = "Tiêu đề", Control = InputControlType.TextBox)]
        public string? Title { get; set; }

        [Field(Title = "Mô tả", Control = InputControlType.TextArea)]
        public string? Description { get; set; }

        [Field(Title = "Nội dung", Control = InputControlType.RichTextBox)]
        public string? Content { get; set; }

        [Field(Title = "Hình ảnh", Control = InputControlType.Image)]
        public string? Image { get; set; }

        [Field(Title = "Đường dẫn", Control = InputControlType.Link)]
        public string? Link { get; set; }

        [Field(Title = "Số lượng", Control = InputControlType.Number)]
        public int Count { get; set; }

        [Field(Title = "Kích hoạt", Control = InputControlType.CheckBox)]
        public bool IsActive { get; set; }

        // Nested class for complex objects
        public class Section
        {
            [Field(Title = "Tiêu đề section", Control = InputControlType.TextBox)]
            public string? Title { get; set; }

            [Field(Title = "Nội dung section", Control = InputControlType.TextArea)]
            public string? Content { get; set; }

            [Field(Title = "Hình ảnh section", Control = InputControlType.Image)]
            public string? Image { get; set; }
        }

        // Single nested object
        [Field(Title = "Section chính")]
        public Section? MainSection { get; set; }

        // Array of simple types
        [Field(Title = "Danh sách tags")]
        public string[]? Tags { get; set; }

        // Array of nested objects
        [Field(Title = "Danh sách sections", ChildTitle = "Section")]
        public Section[]? Sections { get; set; }

        // Optional: Default values for component
        // This will be used as JsonDefault in pp_compt table
        public static TemplateViewModel Default => new TemplateViewModel
        {
            Title = "Tiêu đề mặc định",
            Description = "Mô tả mặc định",
            Count = 10,
            IsActive = true,
            Tags = new[] { "tag1", "tag2" },
            MainSection = new Section
            {
                Title = "Section mặc định",
                Content = "Nội dung mặc định"
            },
            Sections = new Section[]
            {
                new Section { Title = "Section 1", Content = "Nội dung 1" },
                new Section { Title = "Section 2", Content = "Nội dung 2" }
            }
        };
    }
}

