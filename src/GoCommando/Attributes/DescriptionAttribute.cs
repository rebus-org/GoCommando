using System;

namespace GoCommando.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class DescriptionAttribute : Attribute
    {
        readonly string text;

        public DescriptionAttribute(string text)
        {
            this.text = text;
        }

        public string Text
        {
            get { return text; }
        }
    }
}