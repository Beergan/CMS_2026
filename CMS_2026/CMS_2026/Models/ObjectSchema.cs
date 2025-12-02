using System.Collections.Generic;

namespace CMS_2026.Models
{
    public class ObjectSchema
    {
        public string? PropName { get; set; }
        public string? Title { get; set; }
        public string? ChildTitle { get; set; }
        public List<ComponentField> SingleFieldTypes { get; set; } = new();
        public List<ComponentField> ArrayFieldTypes { get; set; } = new();
        public List<ObjectSchema> SingleObjectTypes { get; set; } = new();
        public List<ObjectSchema> ArrayObjectTypes { get; set; } = new();
    }
}

