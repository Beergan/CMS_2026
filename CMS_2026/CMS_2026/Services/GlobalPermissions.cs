using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CMS_2026.Attributes;
using CMS_2026.Models;

namespace CMS_2026.Services
{
    public class GlobalPermissions
    {
        public static Dictionary<FeatureModel, Tuple<long, string, string>[]> Dictionary = new();

        public static void Add(FeatureModel key, Tuple<long, string, string>[] values)
        {
            Dictionary[key] = values;
        }

        public static void RangerRegister(Type enumParent)
        {
            Dictionary.Clear();
            var enumTypes = enumParent.GetNestedTypes()
                .Where(t => t.IsEnum && t.GetCustomAttribute<FeatureAttribute>() != null)
                .ToList();

            if (enumTypes == null) return;

            foreach (var enumType in enumTypes)
            {
                Register(enumType);
            }
        }

        public static void Register(Type enumType)
        {
            var featureAttribute = enumType.GetCustomAttribute<FeatureAttribute>();
            if (featureAttribute == null) return;

            var items = new List<Tuple<long, string, string>>();

            foreach (var functionName in Enum.GetNames(enumType))
            {
                var member = enumType.GetMember(functionName);
                var obsoleteAttribute = member[0].GetCustomAttribute<ObsoleteAttribute>();
                if (obsoleteAttribute != null) continue;

                var funcAttribute = member[0].GetCustomAttribute<FunctionAttribute>();
                if (funcAttribute == null) continue;

                var permissionNo = Convert.ToInt64(Enum.Parse(enumType, functionName, false));
                var permissionValue = Convert.ToInt64(Math.Pow(2, permissionNo));
                items.Add(new Tuple<long, string, string>(
                    permissionValue,
                    funcAttribute.TextEn ?? string.Empty,
                    funcAttribute.TextVi ?? string.Empty));
            }

            var feature = new FeatureModel
            {
                Name = featureAttribute.Name,
                TextEn = featureAttribute.TextEn,
                TextVi = featureAttribute.TextVi
            };
            GlobalPermissions.Add(feature, items.ToArray());
        }
    }
}

