using System;
using System.Collections;
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

        Parameter CreateParameter(PropertyInfo propertyInfo, HelperContext context)
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

            parameter.PropertyInfo = propertyInfo;
            parameter.ArgumentAttribute = argumentAttribute;

            foreach(var example in propertyInfo.GetAttributes<ExampleAttribute>())
            {
                parameter.Examples.Add(example);
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
        public Parameter()
        {
            Examples = new List<ExampleAttribute>();
        }

        public string Description { get; set; }

        public string Name { get; set; }
        public string Shorthand { get; set; }

        public int Position { get; set; }

        public PropertyInfo PropertyInfo { get; set; }
        public ArgumentAttribute ArgumentAttribute { get; set; }

        public List<ExampleAttribute> Examples { get; set; }
    }
}