using System;

namespace GoCommando
{
    /// <summary>
    /// Apply this attribute to a property of a command class (i.e. one that implements <see cref="ICommand"/>)
    /// </summary>
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