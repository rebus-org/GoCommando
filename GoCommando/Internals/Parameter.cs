using System;
using System.Reflection;

namespace GoCommando.Internals
{
    class Parameter
    {
        public PropertyInfo PropertyInfo { get; }
        public string Name { get; }
        public string Shortname { get; }
        public bool Optional { get; }

        public Parameter(PropertyInfo propertyInfo, string name, string shortname, bool optional)
        {
            PropertyInfo = propertyInfo;
            Name = name;
            Shortname = shortname;
            Optional = optional;
        }

        public bool MatchesKey(string key)
        {
            return key == Name
                   || (Shortname != null && key == Shortname);
        }

        public void SetValue(object commandInstance, string value)
        {
            try
            {
                var valueInTheRightType = PropertyInfo.PropertyType == typeof (bool)
                    ? true
                    : Convert.ChangeType(value, PropertyInfo.PropertyType);

                PropertyInfo.SetValue(commandInstance, valueInTheRightType);
            }
            catch (Exception exception)
            {
                throw new FormatException($"Could not set value '{value}' on property named '{PropertyInfo.Name}' on {PropertyInfo.DeclaringType}", exception);
            }
        }
    }
}