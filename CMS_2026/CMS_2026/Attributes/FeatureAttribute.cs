using System;

namespace CMS_2026.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Enum)]
    public class FeatureAttribute : Attribute
    {
        public string? Name { get; set; }
        public string? TextEn { get; set; }
        public string? TextVi { get; set; }
    }
}

