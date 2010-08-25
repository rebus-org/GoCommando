using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace GoCommando.Extensions
{
    public static class AttributeExtensions
    {
        public static void WithAttributes<TAttribute>(this ICustomAttributeProvider customAttributeProvider, Action<TAttribute> handleAttribute)
        {
            GetAttributes<TAttribute>(customAttributeProvider)
                .ToList()
                .ForEach(handleAttribute);
        }

        public static IList<TAttribute> GetAttributes<TAttribute>(this ICustomAttributeProvider customAttributeProvider)
        {
            return customAttributeProvider.GetCustomAttributes(typeof (TAttribute), false)
                .Cast<TAttribute>()
                .ToList();
        }

        public static bool HasAttribute<TAttribute>(this ICustomAttributeProvider customAttributeProvider)
        {
            return GetAttributes<TAttribute>(customAttributeProvider).Any();
        }
    }
}