using System;
using System.Collections.Generic;
using System.Linq;

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

        public static IEnumerable<TAttribute> GetAttributes<TAttribute>(this Type type)
        {
            return type.GetCustomAttributes(typeof (TAttribute), false)
                .Cast<TAttribute>();
        }
    }
}