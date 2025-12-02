using System;

namespace CMS_2026.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public class FunctionAttribute : Attribute
    {
        public string? TextEn { get; set; }
        public string? TextVi { get; set; }
    }
}

