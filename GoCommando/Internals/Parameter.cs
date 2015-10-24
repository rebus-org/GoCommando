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
        public bool AllowAppSetting { get; }
        public bool AllowConnectionString { get; }
        public bool AllowEnvironmentVariable { get; }
        public string[] ExampleValues { get; }

        public bool IsFlag => PropertyInfo.PropertyType == typeof(bool);

        public bool HasDefaultValue => DefaultValue != null;

        public Parameter(PropertyInfo propertyInfo, string name, string shortname, bool optional, string descriptionText, IEnumerable<string> exampleValues, string defaultValue, bool allowAppSetting, bool allowConnectionString, bool allowEnvironmentVariable)
        {
            PropertyInfo = propertyInfo;
            Name = name;
            Shortname = shortname;
            Optional = optional;
            DescriptionText = GetText(descriptionText, allowAppSetting, allowConnectionString, allowEnvironmentVariable);
            DefaultValue = defaultValue;
            AllowAppSetting = allowAppSetting;
            AllowConnectionString = allowConnectionString;
            AllowEnvironmentVariable = allowEnvironmentVariable;
            ExampleValues = exampleValues.ToArray();
        }

        private string GetText(string descriptionText, bool allowAppSetting, bool allowConnectionString, bool allowEnvironmentVariable)
        {
            if (!allowAppSetting && !allowConnectionString && !allowEnvironmentVariable)
            {
                return $"{descriptionText ?? ""}";
            }

            var autoBindings = new List<string>();

            if (allowEnvironmentVariable)
            {
                autoBindings.Add("ENV");
            }

            if (allowAppSetting)
            {
                autoBindings.Add("APP");
            }

            if (allowConnectionString)
            {
                autoBindings.Add("CONN");
            }

            return $"{descriptionText ?? ""} ({string.Join(", ", autoBindings)})";
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
                var valueInTheRightType = PropertyInfo.PropertyType == typeof(bool)
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