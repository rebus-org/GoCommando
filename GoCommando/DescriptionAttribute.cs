using System;

namespace GoCommando
{
    /// <summary>
    /// Apply this attribute to a property of a command class (which is also decorated with <see cref="ParameterAttribute"/>) in
    /// order to provide a description of the parameter
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class DescriptionAttribute : Attribute
    {
        public string DescriptionText { get; }

        public DescriptionAttribute(string descriptionText)
        {
            DescriptionText = descriptionText;
        }
    }
}