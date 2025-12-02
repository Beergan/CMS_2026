using System;

namespace CMS_2026.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ComponentAttribute : Attribute
    {
        public string? Type { get; set; }
        public string? PathPostfix { get; set; }
        public string? ComptName { get; set; }
        public string? NodeType { get; set; }
        public string PageType { get; set; } = "single";
    }
}

