using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace GoCommando.Internals
{
    class Parameter : IEquatable<Parameter>
    {
        public PropertyInfo PropertyInfo { get; }
        public string Name { get; }
        public string Shortname { get; }
        public bool Optional { get; }
        public string DescriptionText { get; }
        public string DefaultValue { get; }
        public string[] ExampleValues { get; }

        public bool IsFlag => PropertyInfo.PropertyType == typeof (bool);

        public bool HasDefaultValue => DefaultValue != null;

        public Parameter(PropertyInfo propertyInfo, string name, string shortname, bool optional, string descriptionText, IEnumerable<string> exampleValues, string defaultValue)
        {
            PropertyInfo = propertyInfo;
            Name = name;
            Shortname = shortname;
            Optional = optional;
            DescriptionText = descriptionText ?? "(no description given)";
            DefaultValue = defaultValue;
            ExampleValues = exampleValues.ToArray();
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

        public void ApplyDefaultValue(ICommand commandInstance)
        {
            if (!HasDefaultValue)
            {
                throw new InvalidOperationException($"Cannot apply default value of '{Name}' parameter because it has no default!");
            }

            SetValue(commandInstance, DefaultValue);
        }

        public bool Equals(Parameter other)
        {
            return Name.Equals(other.Name);
        }
    }
}