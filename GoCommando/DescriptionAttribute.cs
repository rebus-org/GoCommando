using System;

namespace GoCommando
{
    /// <summary>
    /// Apply this attribute to a property of a command class (which is also decorated with <see cref="ParameterAttribute"/>) in
    /// order to provide a description of the parameter
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class)]
    public class DescriptionAttribute : Attribute
    {
        /// <summary>
        /// Gets the description text
        /// </summary>
        public string DescriptionText { get; }

        /// <summary>
        /// Constructs the attribute
        /// </summary>
        public DescriptionAttribute(string descriptionText)
        {
            DescriptionText = descriptionText;
        }
    }
}