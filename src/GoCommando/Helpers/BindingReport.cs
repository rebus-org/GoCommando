using System.Collections.Generic;
using System.Reflection;

namespace GoCommando.Helpers
{
    public class BindingReport
    {
        readonly List<PropertyInfo> propertiesNotBound = new List<PropertyInfo>();
        readonly List<PropertyInfo> propertiesBound = new List<PropertyInfo>();

        public List<PropertyInfo> PropertiesNotBound
        {
            get { return propertiesNotBound; }
        }

        public List<PropertyInfo> PropertiesBound
        {
            get { return propertiesBound; }
        }
    }
}