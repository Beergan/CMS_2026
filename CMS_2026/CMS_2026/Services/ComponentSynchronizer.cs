using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using CMS_2026.Attributes;
using CMS_2026.Data.Entities;
using CMS_2026.Models;
using CMS_2026.Services;
using CMS_2026.ViewModels;
using Newtonsoft.Json;

namespace CMS_2026.Services
{
    public class ComponentSynchronizer
    {
        /// <summary>
        /// Run synchronization: Scan ViewModels and create component schemas
        /// Similar to everflorcom_new ComponentSynchronizer.Run()
        /// </summary>
        public static List<PP_Compt> Run(IDataService dataService)
        {
            var syncedComponents = new List<PP_Compt>();

            // Get all ViewModel types from ViewModels namespace
            // Auto-detect: All classes ending with "ViewModel" or having ComponentAttribute
            var viewModelTypes = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => t.Namespace == "CMS_2026.ViewModels" && 
                           t.IsClass && 
                           !t.IsAbstract &&
                           !t.IsNested &&
                           (t.Name.EndsWith("ViewModel") || t.GetCustomAttribute<ComponentAttribute>() != null))
                .ToList();

            foreach (var viewModelType in viewModelTypes)
            {
                try
                {
                    // Get component attribute (optional - can be null)
                    var comptAttrb = GetComponentAttribute(viewModelType);
                    
                    // Determine component key from type name
                    string finalComptKey = GetComptKeyFromType(viewModelType);
                    
                    if (string.IsNullOrEmpty(finalComptKey))
                        continue;

                    // Check if ViewModel has any Field attributes (skip if no fields)
                    var hasFields = viewModelType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                        .Any(p => p.GetCustomAttribute<FieldAttribute>() != null);
                    
                    if (!hasFields)
                    {
                        // Skip ViewModels without Field attributes
                        System.Diagnostics.Debug.WriteLine($"Skipping {viewModelType.Name}: No Field attributes found");
                        continue;
                    }

                    // Get JSON schema
                    var jsonSchema = GetJsonSchema(viewModelType);
                    if (string.IsNullOrEmpty(jsonSchema))
                        continue;

                    // Get JSON default value
                    var jsonDefaultValue = GetJsonDefaultValue(viewModelType);

                    // Check if component exists
                    var compt = dataService.GetOne<PP_Compt>(t => t.ComptKey == finalComptKey);

                    // Auto-detect component type if not specified
                    var comptType = comptAttrb?.Type;
                    if (string.IsNullOrEmpty(comptType))
                    {
                        // Auto-detect: If name contains "Layout" or "Footer" or "Header" -> Layout
                        // Otherwise -> Page_Template
                        if (viewModelType.Name.Contains("Layout") || 
                            viewModelType.Name.Contains("Footer") || 
                            viewModelType.Name.Contains("Header"))
                        {
                            comptType = "Layout_Component";
                        }
                        else
                        {
                            comptType = "Page_Template";
                        }
                    }

                    // Auto-detect page type if not specified
                    var pageType = comptAttrb?.PageType ?? "single";

                    if (compt != null)
                    {
                        // Update existing
                        compt.ComptType = comptType;
                        compt.ComptName = comptAttrb?.ComptName ?? viewModelType.Name.Replace("ViewModel", "");
                        compt.JsonSchema = jsonSchema;
                        if (!string.IsNullOrEmpty(jsonDefaultValue))
                            compt.JsonDefault = jsonDefaultValue;
                        compt.PathPostfix = comptAttrb?.PathPostfix;
                        compt.NodeType = comptAttrb?.NodeType;
                        compt.PageType = pageType;

                        dataService.Update(compt);
                    }
                    else
                    {
                        // Create new
                        compt = new PP_Compt
                        {
                            ComptKey = finalComptKey,
                            ComptType = comptType,
                            ComptName = comptAttrb?.ComptName ?? viewModelType.Name.Replace("ViewModel", ""),
                            JsonSchema = jsonSchema,
                            JsonDefault = jsonDefaultValue,
                            PathPostfix = comptAttrb?.PathPostfix,
                            NodeType = comptAttrb?.NodeType,
                            PageType = pageType
                        };

                        dataService.Insert(compt);
                    }

                    syncedComponents.Add(compt);
                }
                catch (Exception ex)
                {
                    // Log error but continue with other components
                    System.Diagnostics.Debug.WriteLine($"Error syncing {viewModelType.Name}: {ex.Message}");
                }
            }

            return syncedComponents;
        }

        /// <summary>
        /// Get component key from ViewModel type name
        /// Example: HomeViewModel -> home, FooterViewModel -> footer
        /// </summary>
        private static string GetComptKeyFromType(Type type)
        {
            var typeName = type.Name;
            if (typeName.EndsWith("ViewModel"))
            {
                var baseName = typeName.Substring(0, typeName.Length - "ViewModel".Length);
                // Convert to lowercase with first letter lowercase
                return char.ToLowerInvariant(baseName[0]) + baseName.Substring(1);
            }
            return typeName.ToLowerInvariant();
        }
        private static bool IsSimpleType(Type type)
        {
            var underlyingType = Nullable.GetUnderlyingType(type);
            if (underlyingType != null)
                type = underlyingType;

            return type.IsPrimitive ||
                   type.IsEnum ||
                   type == typeof(string) ||
                   type == typeof(decimal) ||
                   type == typeof(DateTime) ||
                   type == typeof(DateTimeOffset) ||
                   type == typeof(TimeSpan) ||
                   type == typeof(Guid);
        }

        private static bool IsEnumerable(Type type)
        {
            if (type.IsArray) return true;
            if (!type.IsGenericType) return false;
            return type.GetInterfaces().Any(ti => ti.IsGenericType && ti.GetGenericTypeDefinition() == typeof(IEnumerable<>));
        }

        public static ObjectSchema GetObjectSchema(Type type, string? propId = null, string? title = null, string? childTitle = null, int depth = 0)
        {
            var schema = new ObjectSchema
            {
                Title = title ?? propId,
                ChildTitle = childTitle ?? (!IsSimpleType(type) ? type.Name : string.Empty),
                PropName = propId
            };

            if (depth > 4) return schema;

            foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (prop.GetMethod?.IsStatic == true) continue;

                var fieldAttrb = prop.GetCustomAttribute<FieldAttribute>();
                if (fieldAttrb == null) continue;

                if (!IsEnumerable(prop.PropertyType))
                {
                    if (IsSimpleType(prop.PropertyType))
                    {
                        schema.SingleFieldTypes.Add(GetField(prop, fieldAttrb));
                    }
                    else
                    {
                        schema.SingleObjectTypes.Add(GetObjectSchema(prop.PropertyType, prop.Name, fieldAttrb.Title, depth: depth + 1));
                    }
                }
                else if (prop.PropertyType.IsArray)
                {
                    var arrayElementType = prop.PropertyType.GetElementType();
                    if (arrayElementType != null)
                    {
                        if (IsSimpleType(arrayElementType))
                        {
                            schema.ArrayFieldTypes.Add(GetField(prop, fieldAttrb));
                        }
                        else
                        {
                            schema.ArrayObjectTypes.Add(GetObjectSchema(arrayElementType, prop.Name, fieldAttrb.Title ?? prop.Name, fieldAttrb.ChildTitle, depth + 1));
                        }
                    }
                }
                else
                {
                    var genericArgs = prop.PropertyType.GetGenericArguments();
                    if (genericArgs.Length > 0)
                    {
                        var elementType = genericArgs[0];
                        if (IsSimpleType(elementType))
                        {
                            schema.ArrayFieldTypes.Add(GetField(prop, fieldAttrb));
                        }
                        else
                        {
                            schema.ArrayObjectTypes.Add(GetObjectSchema(elementType, prop.Name, fieldAttrb.Title ?? prop.Name, fieldAttrb.ChildTitle, depth + 1));
                        }
                    }
                }
            }

            return schema;
        }

        private static ComponentField GetField(PropertyInfo prop, FieldAttribute attrb)
        {
            return new ComponentField
            {
                FieldId = prop.Name,
                FieldTitle = attrb.Title ?? prop.Name,
                Required = attrb.Required,
                ControlType = attrb.Control ?? "textbox",
                PlaceHolder = attrb.PlaceHolder,
                Options = attrb.Options
            };
        }

        public static ComponentAttribute? GetComponentAttribute(Type type)
        {
            return type.GetCustomAttribute<ComponentAttribute>();
        }

        public static string GetJsonSchema(Type type)
        {
            var schema = GetObjectSchema(type);
            return JsonConvert.SerializeObject(schema, Formatting.None, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
        }

        public static string? GetJsonDefaultValue(Type type, IDictionary<string, string>? phrases = null)
        {
            var defaultProp = type.GetProperty("Default", BindingFlags.Public | BindingFlags.Static);
            if (defaultProp == null) return null;

            try
            {
                var defaultValue = defaultProp.GetValue(null);
                if (defaultValue == null) return null;

                // Set phrases if property exists
                var phrasesProp = defaultValue.GetType().GetProperty("Phrases");
                if (phrasesProp != null && phrases != null)
                {
                    phrasesProp.SetValue(defaultValue, phrases);
                }

                return JsonConvert.SerializeObject(defaultValue, Formatting.None, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });
            }
            catch
            {
                return null;
            }
        }
    }
}

