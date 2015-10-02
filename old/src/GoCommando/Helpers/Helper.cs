using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GoCommando.Attributes;
using GoCommando.Extensions;

namespace GoCommando.Helpers
{
    public class Helper
    {
        class HelperContext : IHasPositionerCounter
        {
            public HelperContext()
            {
                Position = 1;
            }

            public int Position { get; set; }
        }

        public List<Parameter> GetParameters(object obj)
        {
            var context = new HelperContext();

            return obj.GetType().GetProperties()
                .Where(ShouldBeIncluded)
                .Select(p => new Parameter(p, context))
                .ToList();
        }

        bool ShouldBeIncluded(PropertyInfo info)
        {
            return info.HasAttribute<ArgumentAttribute>();
        }
    }
}