using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GoCommando.Attributes;
using GoCommando.Extensions;

namespace GoCommando.Helpers
{
    public class Helper
    {
        class HelperContext
        {
            public int Position { get; set; }
        }

        public List<Parameter> GetParameters(object obj)
        {
            var context = new HelperContext();
            return obj.GetType().GetProperties()
                .Where(ShouldBeIncluded)
                .Select(p => CreateParameter(p, context))
                .ToList();
        }

        Parameter CreateParameter(ICustomAttributeProvider propertyInfo, HelperContext context)
        {
            var attributes = propertyInfo.GetAttributes<DescriptionAttribute>();

            var parameter = new Parameter();

            if (attributes.Count == 1)
            {
                parameter.Description = attributes[0].Text;
            }

            var argumentAttribute = propertyInfo.GetAttributes<ArgumentAttribute>().Single();

            if (argumentAttribute is NamedArgumentAttribute)
            {
                var namedArgumentAttribute = (NamedArgumentAttribute)argumentAttribute;
                parameter.Shorthand = namedArgumentAttribute.ShortHand;
                parameter.Name = namedArgumentAttribute.Name;
            }
            else if (argumentAttribute is PositionalArgumentAttribute)
            {
                context.Position++;
                parameter.Position = context.Position;
            }

            return parameter;
        }

        bool ShouldBeIncluded(PropertyInfo info)
        {
            return info.HasAttribute<ArgumentAttribute>();
        }
    }

    public class Parameter
    {
        public string Description { get; set; }

        public string Name { get; set; }
        public string Shorthand { get; set; }

        public int Position { get; set; }
    }
}