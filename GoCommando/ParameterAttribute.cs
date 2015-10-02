using System;

namespace GoCommando
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ParameterAttribute : Attribute
    {
        public string Name { get; }
        public string ShortName { get; }
        public bool Optional { get; }

        public ParameterAttribute(string name, string shortName = null, bool optional = false)
        {
            Name = name;
            ShortName = shortName;
            Optional = optional;
        }
    }
}