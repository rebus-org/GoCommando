using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GoCommando.Attributes;
using GoCommando.Extensions;

namespace GoCommando.Helpers
{
    public class Parameter
    {
        public Parameter(PropertyInfo propertyInfo, IHasPositionerCounter context)
        {
            Examples = new List<ExampleAttribute>();

            var attributes = propertyInfo.GetAttributes<DescriptionAttribute>();

            if (attributes.Count == 1)
            {
                Description = attributes[0].Text;
            }

            var argumentAttribute = propertyInfo.GetAttributes<ArgumentAttribute>().Single();
            
            if (argumentAttribute is NamedArgumentAttribute)
            {
                var namedArgumentAttribute = (NamedArgumentAttribute)argumentAttribute;
                Shorthand = namedArgumentAttribute.ShortHand;
                Name = namedArgumentAttribute.Name;
            }
            else if (argumentAttribute is PositionalArgumentAttribute)
            {
                Position = context.Position++;
            }

            PropertyInfo = propertyInfo;
            ArgumentAttribute = argumentAttribute;

            foreach (var example in propertyInfo.GetAttributes<ExampleAttribute>())
            {
                Examples.Add(example);
            }

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