using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace GoCommando
{
    public static class AttributeExtensions
    {
        public static void WithAttributes<TAttribute>(this Type type, Action<TAttribute> handleAttribute)
        {
            GetAttributes<TAttribute>(type)
                .ToList()
                .ForEach(handleAttribute);
        }

        public static IEnumerable<TAttribute> GetAttributes<TAttribute>(this ICustomAttributeProvider type)
        {
            return type.GetCustomAttributes(typeof (TAttribute), false)
                .Cast<TAttribute>();
        }

        public static bool HasAttribute<TAttribute>(this ICustomAttributeProvider customAttributeProvider)
        {
            return GetAttributes<TAttribute>(customAttributeProvider).Any();
        }
    }
}