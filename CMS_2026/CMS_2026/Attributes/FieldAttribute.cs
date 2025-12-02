using System;

namespace CMS_2026.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class FieldAttribute : Attribute
    {
        public bool Required { get; set; }
        public string? Title { get; set; }
        public string? ChildTitle { get; set; }
        public string? Control { get; set; }
        public string? PlaceHolder { get; set; }
        public string? InputMask { get; set; }
        public string[]? Options { get; set; }
    }
}

