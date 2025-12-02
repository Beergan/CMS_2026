namespace CMS_2026.Models
{
    public class ComponentField
    {
        public bool Required { get; set; }
        public string FieldId { get; set; } = string.Empty;
        public string FieldTitle { get; set; } = string.Empty;
        public string? PlaceHolder { get; set; }
        public string? InputMask { get; set; }
        public string ControlType { get; set; } = string.Empty;
        public string[]? Options { get; set; }
    }
}

