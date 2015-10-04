using System;

namespace GoCommando
{
    /// <summary>
    /// Apply one or more of these to a command property to show examples on how this particular parameter can be used
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class ExampleAttribute : Attribute
    {
        public string ExampleValue { get; }

        public ExampleAttribute(string exampleValue)
        {
            ExampleValue = exampleValue;
        }
    }
}