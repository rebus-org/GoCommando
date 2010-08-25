using System;

namespace GoCommando.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class ExampleAttribute : Attribute
    {
        readonly string text;

        public ExampleAttribute(string text)
        {
            this.text = text;
        }

        public string Text
        {
            get { return text; }
        }
    }
}